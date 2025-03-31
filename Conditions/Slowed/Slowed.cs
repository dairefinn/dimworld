namespace Dimworld;

using Godot;


public partial class Slowed : Condition
{
    public override void OnPhysicsProcess(double delta, IAffectedByConditions target)
    {
        if (target is CharacterBody3D targetCharacterBody3D)
        {
            targetCharacterBody3D.Velocity /= 2;
        }
        else if (target is CharacterBody2D targetCharacterBody2D)
        {
            targetCharacterBody2D.Velocity /= 2;
        }

        base.OnProcess(delta, target);
    }

}
