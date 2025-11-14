using Godot;

public partial class WaveDirector : Node
{
    [Signal] public delegate void WaveStartedEventHandler(int waveIndex);
    [Signal] public delegate void WaveEndedEventHandler(int waveIndex);

    // === REFERENCES ===
    [Export] public NodePath SpawnerPath;
    private EnemySpawner _spawner;

    // === WAVE TIMING ===
    [Export] public float WaveDuration = 30f;
    [Export] public float RestDuration = 6;
    private float _timer;
    private bool _inWave = false;

    // === DIFFICULTY CURVE ===
    [Export] public float BaseSpawnsPerSecond = 1.6f;
    [Export] public float SpawnsPerSecondPerWave = 0.4f;
    [Export] public int MaxAliveBase = 20;
    [Export] public int MaxAlivePerWave = 4;

    // === RUNTIME ===
    private int _waveIndex = 0;
    private float _spawnBudget = 0f;

    // === READY ===
    public override void _Ready()
    {
        _spawner = GetNode<EnemySpawner>(SpawnerPath);
        StartNextWave();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        if (_inWave)
        {
            _timer -= dt;

            // Acculuate spawn budget using current wave's spawn rate
            float sps = GetCurrentSpawnsPerSecond();
            _spawnBudget += sps * dt;

            // Let the spawner convert whole numbers of budget into enemies
            _spawner.MaxAlive = GetCurrentMaxAlive();
            _spawner.ConsumeSpawnBudget(ref _spawnBudget);

            if (_timer <= 0f) EndWave();
        }
        else
        {
            _timer -= dt;
            if (_timer <= 0f) StartNextWave();
        }
    }

    private void StartNextWave()
    {
        _waveIndex++;
        _inWave = true;
        _timer = WaveDuration;
        EmitSignal(SignalName.WaveStarted, _waveIndex);
    }

    private void EndWave()
    {
        _inWave = false;
        _timer = RestDuration;
        _spawnBudget = 0f;
        EmitSignal(SignalName.WaveEnded, _waveIndex);
    }

    private float GetCurrentSpawnsPerSecond() => BaseSpawnsPerSecond + SpawnsPerSecondPerWave * (_waveIndex - 1);
    private int GetCurrentMaxAlive() => MaxAliveBase + MaxAlivePerWave * (_waveIndex - 1);
}