namespace Dimworld.Entities.Objects;

using Dimworld.Core;
using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Factions;
using Dimworld.Core.Interaction;
using Dimworld.Core.Items;
using Godot;


/// <summary>
/// A chest containing an inventory.
/// Used for storing items.
/// </summary>
public partial class Chest : StaticBody2D, ICanBeInteractedWith, IMemorableNode, IHasInventory, IHasFactionAffiliation
{

    [Export] public Inventory Inventory { get; set; }
    [Export] public Faction Affiliation { get; set; }



    /// <summary>
    /// Checks if the chest contains a given item by its reference.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>True if the chest contains the item, false otherwise.</returns>
    public bool ContainsItem(InventoryItem item)
    {
        return Inventory.HasItem(item);
    }

    /// <summary>
    /// Checks if the chest contains a given item by its ID.
    /// </summary>
    /// <param name="itemId">The ID of the item to check for.</param>
    /// <returns>True if the chest contains the item, false otherwise.</returns>
    public bool ContainsItem(string itemId)
    {
        return Inventory.HasItem(itemId);
    }


    // INTERFACES

    public void InteractWith()
    {
        Globals.Instance.InventoryViewer.OpenSecondaryInventory(Inventory);
    }

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
    }

}
