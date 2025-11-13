using Godot;
using System;

public partial class Bullet : Area2D
{
    // === TUNING ===
    [Export] public float Speed = 1400f;                    //  px/sec
    [Export] public float LifeTime = 1.2f;                  // seconds
    [Export] public float Damage = 10f;                     // damage per hit
    [Export] public uint CollisionMaskEnemies = 1 << 2;     // set to player's enemies layer mask

    // === RUNTIME ===
    public Vector2 Direction = Vector2.Right;               // set by Weapon on spawn
    private float _time;

    public override void _Ready()
    {
        // Ensure we only hit intended targets
        CollisionMask = CollisionMaskEnemies;

        // Connect hits
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        GlobalPosition += Direction * Speed * dt;

        _time += dt;
        if (_time >= LifeTime) QueueFree();
    }

    private void OnBodyEntered(Node body) => TryDamage(body);
    private void OnAreaEntered(Area2D area) => TryDamage(area);

    private void TryDamage(Node hit)
    {
        // Look for damagable component on the hit
        if (hit is Node n)
        {
            var damageable = n.GetNodeOrNull<Damageable>(".");  // try same node
            damageable ??= n.GetNodeOrNull<Damageable>("..");   // try parent node

            if (damageable == null)
            {
                foreach (Node c in hit.GetChildren())
                {
                    if (c is Damageable d) { damageable = d; break; }
                }
            }
            if (damageable != null)
            {
                damageable.ApplyDamage(Damage);
                QueueFree(); // simple: 1 hit then despawn
            }
        }
    }
}
