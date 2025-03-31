namespace Dimworld;

using Godot;


public partial class SpeedBoost : Condition
{

    public override void ApplyTo(IAffectedByConditions target)
    {
        base.ApplyTo(target);
        
        if (target is CharacterController characterController)
        {
            characterController.Speed *= 2;
        }
    }

    public override void RemoveFrom(IAffectedByConditions target)
    {
        base.RemoveFrom(target);

        if (target is CharacterController characterController)
        {
            characterController.Speed /= 2;
        }
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
