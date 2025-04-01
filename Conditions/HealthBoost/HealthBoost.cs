namespace Dimworld;

using Godot;


public partial class HealthBoost : Condition
{

    public override bool ApplyTo(IAffectedByConditions target)
    {
        bool addedSuccessfully = base.ApplyTo(target);
        if (!addedSuccessfully) return false;
        
        if (target is CharacterController characterController)
        {
            characterController.Stats.MaxHealth *= 2;
            characterController.Stats.Heal();
        }

        return true;
    }

    public override bool RemoveFrom(IAffectedByConditions target)
    {
        bool removedSuccesfully = base.RemoveFrom(target);
        if (!removedSuccesfully) return false;

        if (target is CharacterController characterController)
        {
            characterController.Stats.MaxHealth /= 2;
        }

        return true;
    }

}
