namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class Inventory : Node
{

    [Export] public Array<InventorySlot> Slots = [];


    [Signal] public delegate void OnUpdatedEventHandler();


    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A boolean indicating whether the item was added successfully.</returns>
    public bool AddItem(InventoryItem item)
    {
        InventorySlot slot = GetFirstSlotWithItem(item, true);
        if (slot == null)
        {
            GD.Print("No slot found for item: " + item.ItemName);
            slot = GetFirstEmptySlot();
        }
        if (slot == null) return false;

        return slot.AddItem(item);
    }

    /// <summary>
    /// Remove an item from the inventory.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>A boolean indicating whether the item was removed successfully.</returns>
    public bool RemoveItem(InventoryItem item)
    {
        if (!HasItem(item)) return false;

        foreach (InventorySlot slot in Slots)
        {
            if (slot.Item == item)
            {
                return slot.RemoveItem();
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the inventory contains an item.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>A boolean indicating whether the item is in the inventory.</returns>
    public bool HasItem(InventoryItem item)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Item == item) return true;
        }
        
        return false;
    }

    /// <summary>
    /// Get the first empty slot in the inventory.
    /// </summary>
    /// <returns>The first empty slot in the inventory.</returns>
    public InventorySlot GetFirstEmptySlot()
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.IsEmpty) return slot;
        }

        return null;
    }

    /// <summary>
    /// Get the first slot with an item in the inventory.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <param name="ignoreFull">Whether to ignore full slots.</param>
    /// <returns>The first slot with the item in the inventory.</returns>
    public InventorySlot GetFirstSlotWithItem(InventoryItem item, bool ignoreFull = false)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.IsEmpty) continue;
            if (slot.Item.id != item.id) continue;
            if (ignoreFull && slot.IsFull) continue;
            return slot;
        }

        return null;
    }

}
