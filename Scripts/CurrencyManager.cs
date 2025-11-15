using Godot;

public partial class CurrencyManager : Node
{
    [Signal] public delegate void CurrencyChangedEventHandler(int current);
    public int Energy { get; private set; } = 0;

    public void Add(int amount)
    {
        Energy = Mathf.Max(0, Energy + amount);
        EmitSignal(SignalName.CurrencyChanged, Energy);
    }

    public void ResetRun()
    {
        Energy = 0;
        EmitSignal(SignalName.CurrencyChanged, Energy);
    }
}