namespace Dimworld;

using Godot;

[GlobalClass]
public partial class InventorySlot : Resource
{

    [Signal] public delegate void OnUpdatedEventHandler();

    [Export] public InventoryItem Item = null;
    [Export] public int Quantity {
        get => _quantity;
        set => SetQuantity(value);
    }
    private int _quantity = 0;

    public bool IsEmpty => Item == null;
    public bool IsFull => Item != null && Item.MaxStackSize == Quantity;

    
    /// <summary>
    /// Add an item to the inventory slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A boolean indicating whether the item was added successfully.</returns>
    public bool AddItem(InventoryItem item)
    {
        if (IsFull) return false;

        if (IsEmpty)
        {
            Item = item;
            Quantity = 0;
        }

        Quantity++;

        EmitSignal(SignalName.OnUpdated);

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

        EmitSignal(SignalName.OnUpdated);

        return true;
    }

    private void SetQuantity(int value)
    {
        if (Item == null) return;
        _quantity = Mathf.Clamp(value, 0, Item.MaxStackSize);
        EmitSignal(SignalName.OnUpdated);
    }

}
