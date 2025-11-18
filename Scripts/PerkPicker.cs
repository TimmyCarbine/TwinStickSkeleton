using Godot;
using System;

public partial class PerkPicker : Control
{
    [Export] public NodePath WaveDirectorPath;
    [Export] public NodePath PerkApplierPath;
    [Export] public NodePath Button1Path, Button2Path, Button3Path;

    private WaveDirector _dir;
    private PerkApplier _applier;
    private Button _b1, _b2, _b3;
    private PerkDef[] _current = new PerkDef[3];
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _dir = GetNode<WaveDirector>(WaveDirectorPath);
        _applier = GetNode<PerkApplier>(PerkApplierPath);
        _b1 = GetNode<Button>(Button1Path);
        _b2 = GetNode<Button>(Button2Path);
        _b3 = GetNode<Button>(Button3Path);

        _dir.WaveEnded += OnWaveEnded;
        _b1.Pressed += () => Choose(0);
        _b2.Pressed += () => Choose(1);
        _b3.Pressed += () => Choose(2);

        _rng.Randomize();
        Hide();
    }

    public override void _EnterTree()
    {
        // Allow this UI (and its children) to process input while the tree is paused
        ProcessMode = Node.ProcessModeEnum.WhenPaused;
    }

    private void OnWaveEnded(int waveIndex)
    {
        _current[0] = RollPerk();
        _current[1] = RollPerk();
        _current[2] = RollPerk();

        _b1.Text = $"{_current[0].Name}\n{_current[0].Desc}";
        _b2.Text = $"{_current[1].Name}\n{_current[1].Desc}";
        _b3.Text = $"{_current[2].Name}\n{_current[2].Desc}";

        Show();
        GetTree().Paused = true;
    }

    private void Choose(int i)
    {
        _applier.Apply(_current[i]);
        Hide();
        GetTree().Paused = false;
    }

    private PerkDef RollPerk()
    {
        int r = _rng.RandiRange(0, 4);
        return r switch
        {
            0 => new PerkDef("Rapid Fire", "+15% Fire Rate", PerkType.FireRateUp, 0.15f),
            1 => new PerkDef("Tight Barrel", "-20% Bullet Spread", PerkType.SpreadDown, 0.20f),
            2 => new PerkDef("Heavy Shot", "+10% Bullet Damage", PerkType.DamageUp, 0.10f),
            3 => new PerkDef("Afterburn", "-20% Dash Cooldown", PerkType.DashCooldownDown, 0.20f),
            4 => new PerkDef("Ignition Overdrive", "+12 Move Speed", PerkType.MoveSpeedUp, 0.12f),
            _ => new PerkDef("Reinforced Plating", "+10 Max HP", PerkType.MaxHealthUp, 0.10f),
        };
    }
}