namespace Dimworld;

using Godot;


public partial class SpeedBoost : Condition
{

    public override bool ApplyTo(IAffectedByConditions target)
    {
        bool addedSuccessfully = base.ApplyTo(target);
        if (!addedSuccessfully) return false;
        
        if (target is CharacterController characterController)
        {
            characterController.Speed *= 2;
        }

        return true;
    }

    public override bool RemoveFrom(IAffectedByConditions target)
    {
        bool removedSuccesfully = base.RemoveFrom(target);
        if (!removedSuccesfully) return false;

        if (target is CharacterController characterController)
        {
            characterController.Speed /= 2;
        }

        return true;
    }

    // public override void OnPhysicsProcess(double delta, IAffectedByConditions target)
    // {
    //     if (target is CharacterController characterController)
    //     {
    //         characterController.Speed *= 1.5f;
    //     }
    //     else if (target is CharacterBody3D targetCharacterBody3D)
    //     {
    //         targetCharacterBody3D.Velocity *= 1.5f;
    //     }
    //     else if (target is CharacterBody2D targetCharacterBody2D)
    //     {
    //         targetCharacterBody2D.Velocity *= 1.5f;
    //     }

    //     base.OnProcess(delta, target);
    // }

}
