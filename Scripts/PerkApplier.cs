using Godot;

public partial class PerkApplier : Node
{
    [Export] public NodePath PlayerControllerPath;
    [Export] public NodePath WeaponPath;
    [Export] public NodePath PlayerDamageablePath;

    private PlayerController _pc;
    private Weapon _wpn;
    private Damageable _hp;

    public override void _Ready()
    {
        _pc = GetNode<PlayerController>(PlayerControllerPath);
        _wpn = GetNode<Weapon>(WeaponPath);
        _hp = GetNode<Damageable>(PlayerDamageablePath);
    }

    public void Apply(PerkDef perk)
    {
        switch(perk.Type)
        {
            case PerkType.FireRateUp:           _wpn.FireRate *= 1f + perk.Amount; break;
            case PerkType.SpreadDown:           _wpn.SpreadDegrees *= Mathf.Max(0.01f, 1f - perk.Amount); break;
            case PerkType.DamageUp:             _wpn.BulletDamage *= 1f + perk.Amount; break;
            case PerkType.DashCooldownDown:     _pc.DashCooldown *= (float)Mathf.Max(0.05, 1f - perk.Amount); break;
            case PerkType.MoveSpeedUp:          _pc.MoveSpeed *= 1f + perk.Amount; break;
            case PerkType.MaxHealthUp:
                float add = perk.Amount;
                _hp.MaxHealth += add;
                _hp.CurrentHealth += add;
                _hp.EmitSignal("HealthChanged", _hp.CurrentHealth, _hp.MaxHealth);
                break;
        }
    }
}