using Godot;

public partial class Weapon : Node2D
{
    // === BULLET PREFAB & MUZZLE ===
    [Export] public PackedScene BulletScene;
    [Export] public NodePath MuzzlePath;

    // === TUNING ===
    [Export] public float FireRate = 8f;
    [Export] public float BulletSpeed = 1400f;
    [Export] public float BulletLifeTime = 1.2f;
    [Export] public float BulletDamage = 10f;
    [Export] public float SpreadDegrees = 2.0f;
    [Export] public string FireAction = "fire";

    private Marker2D _muzzle;
    private float _cooldown;
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _muzzle = GetNode<Marker2D>(MuzzlePath);
        _rng.Randomize();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (_cooldown > 0) _cooldown -= dt;

        // Hold to fire
        if (Input.IsActionPressed(FireAction) && _cooldown <= 0f)
        {
            FireOne();
            _cooldown = 1f / FireRate;
        }

        // Rotate Gun to face mouse (optional, nice for readability)
        LookAt(GetGlobalMousePosition());
    }

    private void FireOne()
    {
        if (BulletScene == null || _muzzle == null) return;

        // Direction to mouse with slight spread
        Vector2 dir = (GetGlobalMousePosition() - _muzzle.GlobalPosition).Normalized();
        float spreadRad = Mathf.DegToRad(SpreadDegrees);
        float jitter = _rng.RandfRange(-spreadRad, spreadRad);
        dir = dir.Rotated(jitter);

        var bullet = BulletScene.Instantiate<Bullet>();
        bullet.GlobalPosition = _muzzle.GlobalPosition;
        bullet.Direction = dir;

        // Push tuning into the bullet
        bullet.Speed = BulletSpeed;
        bullet.LifeTime = BulletLifeTime;
        bullet.Damage = BulletDamage;

        GetTree().CurrentScene.AddChild(bullet);
    }
}