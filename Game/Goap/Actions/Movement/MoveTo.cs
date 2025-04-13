namespace Dimworld.GOAP.Actions;

using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.GOAP;
using Dimworld.Core.Items;
using Godot;
using Godot.Collections;


// TODO: Unfinished action
public partial class MoveTo : GoapAction
{

    public Vector2 TargetPosition { get; set; }


    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"current_position", Vector2.Zero}
        });
    }

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        if (goapAgent is not IHasInventory hasInventory) return false; // Can only be performed by a character controller
        if (hasInventory.Inventory.IsFull()) return false; // Cannot pick up a sword if the agent's inventory is full

        // Get the TargetItemId from the world state
        if (!worldState.ContainsKey("target_position")) return false; // Must have a target item set
        Vector2? targetPosition = worldState.GetKey("target_position").AsVector2();
        if (targetPosition == null) return false; // Must have a target item set

        TargetPosition = targetPosition.Value;

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not IHasNavigation hasNavigation) return false; // Must be a character controller

        if (!hasNavigation.IsTargetingPoint(TargetPosition))
        {
            if (goapAgent is ICanSpeak canSpeak)
            {
                canSpeak.Say("I'm moving to " + TargetPosition);
            }
        }

        hasNavigation.NavigateTo(TargetPosition);

        return hasNavigation.IsTargetReached();
    }

}
