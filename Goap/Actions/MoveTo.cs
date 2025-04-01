namespace Dimworld;

using Godot;
using System;


public partial class MoveTo : GoapAction
{

    private Vector2 targetPosition;


    public override void PreCanPerform(IGoapAgent goapAgent)
    {
        if (goapAgent is not CharacterController characterController) return;

        targetPosition = characterController.GlobalPosition;
    }

}
