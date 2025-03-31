namespace Dimworld;

using Godot;


public partial class HealthBoost : Condition
{

    public override void ApplyTo(IAffectedByConditions target)
    {
        base.ApplyTo(target);
        
        if (target is CharacterController characterController)
        {
            characterController.Stats.MaxHealth *= 2;
            characterController.Stats.Heal();
        }
    }

    public override void RemoveFrom(IAffectedByConditions target)
    {
        base.RemoveFrom(target);

        if (target is CharacterController characterController)
        {
            characterController.Stats.MaxHealth /= 2;
        }
    }

}
