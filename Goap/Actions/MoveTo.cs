namespace Dimworld.GOAP.Actions;

using Dimworld.Core.Characters;
using Dimworld.Core.GOAP;
using Godot;
using Godot.Collections;


// TODO: Unfinished action
public partial class MoveTo : GoapAction
{

    public Vector2? TargetPosition { get; set; }


    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"current_position", Vector2.Zero}
        });
    }

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        if (goapAgent is not CharacterController characterController) return false; // Can only be performed by a character controller
        if (characterController.Inventory.IsFull()) return false; // Cannot pick up a sword if the agent's inventory is full

        // Get the TargetItemId from the world state
        if (!worldState.ContainsKey("target_position")) return false; // Must have a target item set
        TargetPosition = worldState.GetKey("target_position").AsVector2();
        if (TargetPosition == null) return false; // Must have a target item set

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false; // Must be a character controller

        if (characterController.NavigationAgent.TargetPosition != TargetPosition)
        {
            characterController.Say("I'm moving to " + TargetPosition);
        }

        characterController.NavigateTo(TargetPosition.Value);

        return characterController.NavigationAgent.IsTargetReached();
    }

}
