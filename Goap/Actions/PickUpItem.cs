namespace Dimworld.GOAP.Actions;

using Dimworld.Items;
using Dimworld.Memory.MemoryEntries;
using Godot;
using Godot.Collections;
using System.Linq;


public partial class PickUpItem : GoapAction
{

    public string ItemId { get; set; }


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

        NodeLocation[] containerLocations = characterController.MemoryHandler.GetMemoriesOfType<NodeLocation>().Where(node => node.Node is IHasInventory hasInventory && hasInventory.CanTakeFromInventory).ToArray();
        if (containerLocations.Length == 0) return null; // Must have at least one chest location in memory

        InventoryContents[] inventoryContents = characterController.MemoryHandler.GetMemoriesOfType<InventoryContents>();

        System.Collections.Generic.List<IHasInventory> containersToSearch = containerLocations
            .Where(location => {
                if (location.Node == null) return false; // Skip null nodes
                if (location.Node is not IHasInventory container) return false; // Must be a container
                if (!characterController.CanReachPoint(location.Position)) return false; // Must be reachable
            
                InventoryContents contentsMemory = inventoryContents.FirstOrDefault(memory => memory.Node == container);

                if (contentsMemory == null) return false;
                if (!contentsMemory.Inventory.HasItem(itemId)) return false;

                return true;
            })
            .OrderBy(memory => memory.Position.DistanceTo(goapAgent.GlobalPositionThreadSafe))
            .Select(memory => memory.Node as IHasInventory)
            .ToList();

        if (containersToSearch.Count == 0) return null; // Must have at least one container to check

        return containersToSearch.First();
    }

}
