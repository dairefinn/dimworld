namespace Dimworld.GOAP.Actions;

using Godot;
using Godot.Collections;
using Dimworld.MemoryEntries;
using System.Linq;

public partial class PickUpItem : GoapAction
{

    public string ItemId { get; set; }
    // public IHasInventory TargetContainer { get; set; }


    public override void PreEvaluate(GoapState worldState, GoapState desiredState)
    {
        // Set the ItemId from the world state
        if (desiredState.ContainsKey("has_items"))
        {
            Array itemsArray = desiredState.GetKey("has_items").AsGodotArray();
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
            {"can_reach_items", itemIds}
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
            {"has_items", itemIds}
        });
    }

    // public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    // {
    //     if (goapAgent is not CharacterController characterController) return false; // Can only be performed by a character controller
    //     if (characterController.Inventory.IsFull()) return false; // Cannot pick up a sword if the agent's inventory is full
    //     if (ItemId == null) return false; // Must have a target item set

    //     // Get the target container from the world state
    //     if (worldState.ContainsKey("target_container"))
    //     {
    //         GodotObject containerObject = worldState.GetKey("target_container").AsGodotObject();
    //         if (containerObject is IHasInventory container) {
    //             TargetContainer = container;
    //         }
    //     }
    //     if (TargetContainer == null) return false; // Must have a target container set

    //     return true;
    // }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false; // Must be a character controller

        characterController.Say("I'm grabbing my sword.");

        InventorySlot agentSlot = characterController.Inventory.GetFirstEmptySlot();

        IHasInventory closestContainerWithItem = GetClosestContainerWithItem(goapAgent, ItemId);

        InventorySlot chestSlot = closestContainerWithItem.Inventory.GetFirstSlotWithItem(ItemId);
        agentSlot.SwapWithExisting(chestSlot);
        characterController.SetInventoryState();
        characterController.MemoryHandler.AddMemory(new InventoryContents(closestContainerWithItem));

        return true;
    }

    private IHasInventory GetClosestContainerWithItem(IGoapAgent goapAgent, string itemId)
    {
        if (goapAgent is not CharacterController characterController) return null; // Must be a character controller

        Vector2 agentPosition = characterController.GlobalPosition;

        // Get any nearby containers that can be reached and have the item
        System.Collections.Generic.List<IHasInventory> containers = [..
            characterController.DetectionHandler.GetDetectedInstancesImplementing<IHasInventory>()
            .Where(container => {
                if (container is not Node2D node2D) return false; // Must be a node2D
                if (!characterController.CanReachPoint(node2D.GlobalPosition)) return false;
                if (!container.Inventory.HasItem(ItemId)) return false;
                return true;
            })
            .ToArray()
        ];

        if (containers.Count == 0) return null; // No containers found

        IHasInventory closestContainer = containers[0];
        for(int i = 1; i < containers.Count; i++)
        {
            GD.Print($"Checking container {i}");
            IHasInventory container = containers[i];
            if (container == null) continue; // Skip null containers
            if (container is not Node2D node2D) continue; // Must be a node2D
            if (agentPosition.DistanceTo(node2D.GlobalPosition) < agentPosition.DistanceTo(node2D.GlobalPosition))
            {
                closestContainer = container;
            }
        }

        return closestContainer;
    }

}
