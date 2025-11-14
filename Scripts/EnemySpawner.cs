using Godot;
using System;
using System.Linq;

public partial class EnemySpawner : Node
{
    // === REFERENCES ===
    [Export] public NodePath PlayerPath;
    private CharacterBody2D _player;

    // === ENEMY POOL ===
    [Export] public PackedScene[] EnemyScenes;
    [Export] public float[] EnemyWeights;

    // === SPAWN GEOMETRY ===
    [Export] public float SpawnRingRadius = 600f;
    [Export] public float MinSeparationFromPlayer = 220f;

    // === LIMITS ===
    [Export] public int MaxAlive = 30;

    private RandomNumberGenerator _rng = new();

    // === START ===
    public override void _Ready()
    {
        _player = GetNode<CharacterBody2D>(PlayerPath);
        _rng.Randomize();
    }

    // Called by WaveDirctor each frame witha  "spawn budget"
    // Example: if budget accumulates to >= 1, we spawn one enemy, etc
    public void ConsumeSpawnBudget(ref float budget)
    {
        if (EnemyScenes == null || EnemyScenes.Length == 0) return;

        while (budget >= 1f && GetAliveCount() < MaxAlive)
        {
            SpawnOne();
            budget -= 1f;
        }
    }

    private void SpawnOne()
    {
        var scene = PickEnemySceneWeighted();
        if (scene == null) return;

        var enemy = scene.Instantiate<CharacterBody2D>();
        GetTree().CurrentScene.AddChild(enemy);

        // Pick a point in a ring around the player, avoiding too close spawns
        Vector2 center = _player.GlobalPosition;
        Vector2 pos;
        int guard = 0;
        do
        {
            float ang = _rng.RandfRange(0f, Mathf.Tau);
            float r = SpawnRingRadius + _rng.RandfRange(-60f, 60f);
            pos = center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r;
            guard++;
        }
        while (guard < 8 && pos.DistanceTo(center) < MinSeparationFromPlayer);

        enemy.GlobalPosition = pos;
    }

    private int GetAliveCount() => GetTree().GetNodesInGroup("enemies").Count;

    private PackedScene PickEnemySceneWeighted()
    {
        if (EnemyWeights == null || EnemyWeights.Length != EnemyScenes.Length) return EnemyScenes[0];

        float sum = 0f;
        foreach (var w in EnemyWeights) sum += Math.Max(0.0001f, w);

        float roll = _rng.Randf() * sum;
        for (int i = 0; i < EnemyScenes.Length; i++)
        {
            float w = Math.Max(0.0001f, EnemyWeights[i]);
            if (roll < w) return EnemyScenes[i];
            roll -= w;
        }
        return EnemyScenes[^1];
    }
}