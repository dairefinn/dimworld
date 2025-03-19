namespace Dimworld;

using Godot;

[GlobalClass]
public partial class InventorySlot : Resource
{
    [Export] public InventoryItem Item = null;
    [Export] public int Quantity = 0;

    public bool IsEmpty => Item == null;
    public bool IsFull => Item != null && Item.MaxStackSize == Quantity;

    
    /// <summary>
    /// Add an item to the inventory slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A boolean indicating whether the item was added successfully.</returns>
    public bool AddItem(InventoryItem item)
    {
        if (IsEmpty)
        {
            Item = item;
            Quantity = 1;
            return true;
        }

        if (item != Item) return false;

        if (IsFull) return false;

        Quantity++;
        return true;
    }

    /// <summary>
    /// Remove an item from the inventory slot.
    /// </summary>
    /// <returns>A boolean indicating whether the item was removed successfully.</returns>
    public bool RemoveItem()
    {
        if (IsEmpty) return false;

        Quantity--;

        if (Quantity == 0)
        {
            Item = null;
        }

        return true;
    }
}