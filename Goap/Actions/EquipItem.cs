namespace Dimworld.GOAP.Actions;

using Godot;
using Godot.Collections;


public partial class EquipItem : GoapAction
{

    public string ItemId { get; set; }


    public override void PreEvaluate(GoapState worldState, GoapState desiredState)
    {
        if (desiredState == null) return;

        // Set the ItemId from the world state
        if (desiredState.ContainsKey("has_items_equipped"))
        {
            Array itemsArray = desiredState.GetKey("has_items_equipped").AsGodotArray();
            if (itemsArray.Count > 0)
            {
                ItemId = itemsArray[0].AsString();
            }
        }
    }

    public override GoapState GetPreconditions()
    {
        Array<Variant> itemIds = [];

        if (ItemId != null)
        {
            itemIds.Add(ItemId);
        }

        return new GoapState(new Dictionary<string, Variant> {
            {"has_items", itemIds}
        });
    }

    public override GoapState GetEffects()
    {
        Array<Variant> itemIds = [];

        if (ItemId != null)
        {
            itemIds.Add(ItemId);
        }

        return new GoapState(new Dictionary<string, Variant> {
            {"has_items_equipped", itemIds}
        });
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false;

        characterController.Say("I'm equipping my sword.");

        Array equippedItems = [];
        
        if (goapAgent.WorldState.ContainsKey("has_items_equipped"))
        {
            equippedItems = goapAgent.WorldState.GetKey("has_items_equipped").AsGodotArray();
        }

        equippedItems.Add(ItemId);
        goapAgent.WorldState.SetKey("has_items_equipped", equippedItems);

        // TODO: Agent should equip the sword but only the player can equip stuff currently

        return true;
    }

}
