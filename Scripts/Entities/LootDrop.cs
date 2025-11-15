using Godot;

public partial class LootDrop : Node
{
    [Export] public NodePath DamageablePath;
    [Export] public PackedScene PickupScene;
    [Export] public float DropChance = 0.65f;
    [Export] public int MinValue = 1;
    [Export] public int MaxValue = 3;

    private Damageable _dmg;
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        _dmg = GetNode<Damageable>(DamageablePath);
        _dmg.Died += OnDied;
    }

    private void OnDied()
    {
        if (PickupScene == null) return;
        if (_rng.Randf() > DropChance) return;

        var pos =
            (GetOwner() as Node2D)?.GlobalPosition ??
            (GetParent() as Node2D)?.GlobalPosition ?? Vector2.Zero;

        CallDeferred(nameof(SpawnPickupDeferred), pos);
    }

    private void SpawnPickupDeferred(Vector2 pos)
    {
        var p = PickupScene.Instantiate<Pickup>();
        p.Value = _rng.RandiRange(MinValue, MaxValue);
        GetTree().CurrentScene.AddChild(p);
        p.GlobalPosition = pos;
    }
}