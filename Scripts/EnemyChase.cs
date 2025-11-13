using Godot;

public partial class EnemyChase : CharacterBody2D
{
    [Export] public float Speed = 60f;
    private Node2D _player;

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as Node2D;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;
        Vector2 dir = (_player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = dir * Speed;
        MoveAndSlide();
    }
}