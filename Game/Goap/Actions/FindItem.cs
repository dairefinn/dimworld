namespace Dimworld.GOAP.Actions;

using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Developer;
using Dimworld.Core.Factions;
using Dimworld.Core.GOAP;
using Dimworld.Core.Items;
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
    private bool knowsWhereItemIs = false;
    private IHasInventory currentTarget = null;


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
        if (goapAgent is not IHasMemory hasMemory) return false; // Can only be performed by a character controller
        if (goapAgent is not IHasNavigation hasNavigation) return false; // Can only be performed by a character controller
        if (ItemId == null) return false; // Must have a target item set

        NodeLocation[] containerLocations = hasMemory.MemoryHandler.GetMemoriesOfType<NodeLocation>().Where(node => node.Node is IHasInventory hasInventory && Faction.CanAccessNode(goapAgent, node.Node)).ToArray();
        if (containerLocations.Length == 0) return false; // Must have at least one chest location in memory

        InventoryContents[] inventoryContents = hasMemory.MemoryHandler.GetMemoriesOfType<InventoryContents>();

        foreach (NodeLocation location in containerLocations)
        {
            if (location.Node == null) continue; // Skip null nodes
            if (location.Node is not IHasInventory container) continue; // Must be a container
            if (!hasNavigation.CanReachPoint(location.Position)) return false; // Must be reachable
            
            InventoryContents contentsMemory = inventoryContents.FirstOrDefault(memory => memory.Node == container);

            if (contentsMemory != null)
            {
                if (contentsMemory.Inventory.HasItem(ItemId))
                {
                    containersWithItem.Add(container);
                    knowsWhereItemIs = true;
                }
            }
            else
            {
                containersToSearch.Add(container);
            }
        }

        if (_containersBacklog.Count == 0) return false; // Must have at least one container to check

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (!actionStarted)
        {
            if (goapAgent is ICanSpeak canSpeak)
            {
                if (knowsWhereItemIs)
                {
                    canSpeak.Say($"I know where the item is: {ItemId}.");
                }
                else
                {
                    canSpeak.Say($"I'm looking for: {ItemId}.");
                }
            }
        }
        actionStarted = true;
        
        Vector2 agentPosition = goapAgent.GlobalPositionThreadSafe;

        // Get the next container to search
        if (currentTarget == null)
        {
            if (_containersBacklog.Count == 0) return false; // No containers left to search

            if (containersWithItem.Count > 0)
            {
                DeveloperConsole.Print("Searching for item in containers with known items");
                currentTarget = GetClosestContiner(containersWithItem, agentPosition);
            }
            else if (containersToSearch.Count > 0)
            {
                DeveloperConsole.Print("Searching for item in containers to search");
                currentTarget = GetClosestContiner(containersToSearch, agentPosition);
            }

        }

        if (currentTarget is not Node2D containerNode2D)
        {
            RemoveContainerFromBacklog(currentTarget);
            return false; // No containers left to search
        }

        if (goapAgent is not IHasNavigation hasNavigation) return false;

        hasNavigation.NavigateTo(containerNode2D.GlobalPosition);
        if (hasNavigation.IsTargetReached())
        {
            if (goapAgent is IHasMemory hasMemory)
            {
                hasMemory.MemoryHandler.AddMemory(new InventoryContents(currentTarget));
            }

            if (currentTarget.Inventory.HasItem(ItemId))
            {
                if (goapAgent is ICanSpeak canSpeak)
                {
                    canSpeak.Say($"I found the item: {ItemId}.");
                }
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
                DeveloperConsole.Print("Item not found in container");
                if (goapAgent is ICanSpeak canSpeak)
                {
                    canSpeak.Say($"I couldn't find the item: {ItemId}.");
                }
                containersChecked.Add(currentTarget);
                RemoveContainerFromBacklog(currentTarget);
                currentTarget = null;
                return false; // No items found in container
            }
        }

        return false;
    }

    private void RemoveContainerFromBacklog(IHasInventory container)
    {
        if (containersWithItem.Contains(container))
        {
            containersWithItem.Remove(container);
        }
        else if (containersToSearch.Contains(container))
        {
            containersToSearch.Remove(container);
        }
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
