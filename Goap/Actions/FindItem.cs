namespace Dimworld.GOAP.Actions;

using Dimworld.MemoryEntries;
using Godot;
using Godot.Collections;
using System.Data;
using System.Linq;

public partial class FindItem : GoapAction
{

    public string ItemId { get; set; }

    private System.Collections.Generic.List<IHasInventory> containersBacklog = [];
    private System.Collections.Generic.List<IHasInventory> containersChecked = [];
    private bool actionStarted = false;


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

        // Get any nearby containers that can be reached
        // TODO: Check if contents are known from memory
        containersBacklog = [..
            characterController.DetectionHandler.GetDetectedInstancesImplementing<IHasInventory>()
            .Where(container => {
                if (container is not Node2D node2D) return false; // Must be a node2D
                if (!characterController.CanReachPoint(node2D.GlobalPosition)) return false;
                // if (!container.Inventory.HasItem(ItemId)) return false; // TODO: Check memory
                return true;
            })
            .ToArray()
        ];
        if (containersBacklog.Count == 0) return false;

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
        IHasInventory closestContainer = GetClosestContiner(containersBacklog, agentPosition);

        if (closestContainer == null || closestContainer is not Node2D containerNode2D) {
            GD.Print("No containers found");
            return false; // No containers found
        }

        characterController.NavigateTo(containerNode2D.GlobalPosition);
        if (characterController.NavigationAgent.IsTargetReached())
        {
            characterController.MemoryHandler.AddMemory(InventoryContents.FromNode(closestContainer));

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
                containersBacklog.Remove(closestContainer);
                return false; // No items found in container
            }
        }


        // characterController.MemoryHandler.AddMemory(InventoryContents.FromNode(detectedChest));

        return false;
    }

    private IHasInventory GetClosestContiner(System.Collections.Generic.List<IHasInventory> containers, Vector2 agentPosition)
    {
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
