using Godot;

public partial class RunHud : Control
{
    // === REFERENCES ===
    [Export] public NodePath WaveDirectorPath;
    [Export] public NodePath PlayerDamagablePath;
    [Export] public NodePath WaveLabelPath;
    [Export] public NodePath TimerLabelPath;
    [Export] public NodePath HealthBarPath;
    [Export] public NodePath EnergyLabelPath;

    private CurrencyManager _currencyManager;
    private WaveDirector _director;
    private Node _playerDamageableNode;
    private Label _waveLabel, _timerLabel, _energyLabel;
    private TextureProgressBar _healthBar;

    private int _lastWave = -1;
    private float _lastHp = -1, _lastHpMax = -1;

    public override void _Ready()
    {
        _currencyManager = GetTree().Root.GetNode<CurrencyManager>("/root/CurrencyManager");
        _director = GetNode<WaveDirector>(WaveDirectorPath);
        _playerDamageableNode = GetNode<Damageable>(PlayerDamagablePath);
        _waveLabel = GetNode<Label>(WaveLabelPath);
        _timerLabel = GetNode<Label>(TimerLabelPath);
        _energyLabel = GetNode<Label>(EnergyLabelPath);
        _healthBar = GetNode<TextureProgressBar>(HealthBarPath);

        _director.WaveStarted += OnWaveStarted;
        _director.WaveEnded += OnWaveEnded;
        _currencyManager.CurrencyChanged += OnCurrencyChanged;
        OnCurrencyChanged(_currencyManager.Energy);

        var damageable = _playerDamageableNode as Damageable;
        if (damageable != null)
        {
            damageable.HealthChanged += OnHealthChanged;
            OnHealthChanged(damageable.CurrentHealth, damageable.MaxHealth);
        }

        OnWaveStarted(_director.WaveIndex);
    }

    public override void _Process(double delta)
    {
        UpdateTimer(_director.TimeRemaining, _director.InWave);
    }

    private void OnWaveStarted(int waveIndex)
    {
        if (waveIndex != _lastWave)
        {
            _waveLabel.Text = $"Wave {waveIndex}";
            _lastWave = waveIndex;
        }
    }

    private void OnWaveEnded(int waveIndex)
    {
        _waveLabel.Text = $"Rest (Wave {waveIndex} cleared)";
    }

    private void UpdateTimer(float seconds, bool inWave)
    {
        int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int mm = s / 60;
        int ss = s % 60;
        _timerLabel.Text = inWave
            ? $"Time Left: {mm:00}:{ss:00}"
            : $"Next Wave: {mm:00}:{ss:00}";
    }

    private void OnHealthChanged(float current, float max)
    {
        if (!Mathf.IsEqualApprox(current, _lastHp) || !Mathf.IsEqualApprox(max, _lastHpMax))
        {
            _healthBar.MaxValue = max;
            _healthBar.Value = current;

            _lastHp = current;
            _lastHpMax = max;
        }
    }

    private void OnCurrencyChanged(int current)
    {
        _energyLabel.Text = $"Cores: {current}";
    }

    private static string NodePathToString(NodePath p) => p == null ? "" : p.ToString();
}