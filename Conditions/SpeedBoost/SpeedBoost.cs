namespace Dimworld;

using Godot;


public partial class SpeedBoost : Condition
{
    public override void OnPhysicsProcess(double delta, IAffectedByConditions target)
    {
        if (target is CharacterBody3D targetCharacterBody3D)
        {
            targetCharacterBody3D.Velocity *= 1.5f;
        }
        else if (target is CharacterBody2D targetCharacterBody2D)
        {
            targetCharacterBody2D.Velocity *= 1.5f;
        }

        base.OnProcess(delta, target);
    }
}
