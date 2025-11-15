using Godot;

public partial class Pickup : Area2D
{
    [Export] public int Value = 1;
    [Export] public float MagnetRadius = 180f;
    [Export] public float MagnetSpeed = 550f;
    private CharacterBody2D _player;

    public override void _Ready()
    {
        SetDeferred("monitoring", true);
        SetDeferred("monitorable", true);

        BodyEntered += OnBodyEntered;
        _player = GetTree().GetFirstNodeInGroup("player") as CharacterBody2D;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        if (_player == null) return;
        var toPlayer = _player.GlobalPosition - GlobalPosition;
        if (toPlayer.Length() <= MagnetRadius)
            GlobalPosition += toPlayer.Normalized() * MagnetSpeed * dt;
    }

    private void OnBodyEntered(Node body)
    {
        if (!body.IsInGroup("player")) return;
        var cm = GetTree().Root.GetNodeOrNull<CurrencyManager>("/root/CurrencyManager");
        cm?.Add(Value);
        QueueFree();
    }
}