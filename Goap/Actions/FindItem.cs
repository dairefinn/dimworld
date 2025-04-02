namespace Dimworld.GOAP.Actions;

using Dimworld.MemoryEntries;
using Godot;
using Godot.Collections;
using System.Linq;

public partial class FindItem : GoapAction
{

    public string ItemId { get; set; }

    private System.Collections.Generic.HashSet<IHasInventory> containersWithItem = [];
    private System.Collections.Generic.HashSet<IHasInventory> containersToSearch = [];
    private System.Collections.Generic.HashSet<IHasInventory> containersChecked = [];
    private bool actionStarted = false;


    private System.Collections.Generic.HashSet<IHasInventory> _containersBacklog => [.. containersWithItem, .. containersToSearch];


    public override void PreEvaluate(GoapState worldState, GoapState desiredState)
    {
        // Set the ItemId from the world state
        if (desiredState.ContainsKey("can_reach_items"))
        {
            Array itemsArray = desiredState.GetKey("can_reach_items").AsGodotArray();
            if (itemsArray.Count > 0)
            {
                ItemId = itemsArray[0].AsString();
            }
        }
    }

    public override GoapState GetEffects()
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

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        if (goapAgent is not CharacterController characterController) return false; // Can only be performed by a character controller
        if (ItemId == null) return false; // Must have a target item set

        // Get any containers that are in the character's memory with the item
        foreach(InventoryContents memory in characterController.MemoryHandler.GetMemoriesOfType<InventoryContents>())
        {
            if (memory.Node == null) continue; // Skip null nodes
            if (memory.Inventory == null) continue; // Skip null inventories
            if (!memory.Inventory.HasItem(ItemId)) continue; // Skip inventories that don't have the item
            if (containersChecked.Contains(memory.Node)) continue; // Skip containers that have already been checked
            if (memory.Node is not IHasInventory container) continue; // Must be a container
            if (container is not Node2D node2D) continue; // Must be a node2D
            if (!characterController.CanReachPoint(node2D.GlobalPosition)) return false;
            containersWithItem.Add(container);
        }

        // Get any nearby containers that can be reached
        foreach(Node2D node in characterController.DetectionHandler.GetDetectedInstancesImplementing<IHasInventory>())
        {
            if (node == null) continue; // Skip null nodes
            if (node is not IHasInventory container) continue; // Must be a container
            if (containersChecked.Contains(container)) continue; // Skip containers that have already been checked
            if (container is not Node2D node2D) continue; // Must be a node2D
            if (!characterController.CanReachPoint(node2D.GlobalPosition)) return false;
            containersToSearch.Add(container);
        }

        if (_containersBacklog.Count == 0) return false; // Must have at least one container to check

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false; // Must be a character controller

        if (!actionStarted)
        {
            characterController.Say($"I'm looking for: {ItemId}.");
        }
        actionStarted = true;
        
        Vector2 agentPosition = characterController.GlobalPosition;

        IHasInventory closestContainer = null;
        if (containersWithItem.Count > 0)
        {
            closestContainer = GetClosestContiner(containersWithItem, agentPosition);
        }
        else if (containersToSearch.Count > 0)
        {
            closestContainer = GetClosestContiner(containersToSearch, agentPosition);
        }

        if (closestContainer == null || closestContainer is not Node2D containerNode2D) {
            GD.Print("No containers found");
            return false; // No containers found
        }

        characterController.NavigateTo(containerNode2D.GlobalPosition);
        if (characterController.NavigationAgent.IsTargetReached())
        {
            characterController.MemoryHandler.AddMemory(new InventoryContents(closestContainer));

            if (closestContainer.Inventory.HasItem(ItemId))
            {
                characterController.Say($"I found the item: {ItemId}.");
                Array itemsArray = [];
                
                if (goapAgent.WorldState.ContainsKey("can_reach_items"))
                {
                    itemsArray = goapAgent.WorldState.GetKey("can_reach_items").AsGodotArray();
                }

                itemsArray.Add(ItemId);
                goapAgent.WorldState.SetKey("can_reach_items", itemsArray);
                return true;
            }
            else
            {
                GD.Print("Item not found in container");
                characterController.Say($"I couldn't find the item: {ItemId}.");
                containersChecked.Add(closestContainer);
                containersToSearch.Remove(closestContainer);
                containersWithItem.Remove(closestContainer);
                return false; // No items found in container
            }
        }


        // characterController.MemoryHandler.AddMemory(InventoryContents.FromNode(detectedChest));

        return false;
    }

    private IHasInventory GetClosestContiner(System.Collections.Generic.HashSet<IHasInventory> containers, Vector2 agentPosition)
    {
        if (containers.Count == 0) return null; // No containers found

        IHasInventory closestContainer = null;
        Vector2 closestContainerPosition = Vector2.Zero;
        for(int i = 0; i < containers.Count; i++)
        {
            IHasInventory container = containers.ElementAt(i);
            if (container == null) continue; // Skip null containers
            if (container is not Node2D node2D) continue; // Must be a node2D
            if (agentPosition.DistanceTo(node2D.GlobalPosition) < agentPosition.DistanceTo(closestContainerPosition))
            {
                closestContainer = container;
                closestContainerPosition = node2D.GlobalPosition;
            }
        }

        return closestContainer;
    }

}
