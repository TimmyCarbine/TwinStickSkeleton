using Godot;

public partial class Damageable : Node
{
    [Signal] public delegate void DiedEventHandler();

    [Export] public float MaxHealth = 30f;
    public float CurrentHealth;

    public override void _Ready() => CurrentHealth = MaxHealth;

    public void ApplyDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0f)
        {
            EmitSignal(SignalName.Died);
            // Let the owner free itself or do it here
            (GetOwner() ?? this).QueueFree();
        }
    }
}