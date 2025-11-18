public enum PerkType { FireRateUp, DashCooldownDown, MaxHealthUp, MoveSpeedUp, SpreadDown, DamageUp }

public class PerkDef
{
    public string Name;
    public string Desc;
    public PerkType Type;
    public float Amount;

    public PerkDef(string name, string desc, PerkType type, float amount)
    { Name = name; Desc = desc;  Type = type;  Amount = amount; }
}