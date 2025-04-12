using Dimworld.Core.Modifiers;

namespace Dimworld.Modifiers;


public partial class HealthMultiplyModifier : CharacterStatsModifier
{

    public float Multiplier { get; set; } = 1f;
    public bool HealOnAdd { get; set; } = true;


    public HealthMultiplyModifier(string key, float multiplier) : base(key)
    {
        Multiplier = multiplier;
    }


    public HealthMultiplyModifier SetHealOnAdd(bool healOnAdd)
    {
        HealOnAdd = healOnAdd;
        return this;
    }


    public override void OnAdded(ModifierHandler handler)
    {
        base.OnAdded(handler);

        TargetStats.MaxHealth *= Multiplier;

        if (HealOnAdd)
        {
            TargetStats.Heal();
        }
    }

    public override void OnRemoved(ModifierHandler handler)
    {
        base.OnRemoved(handler);

        TargetStats.MaxHealth /= Multiplier;
    }

}
