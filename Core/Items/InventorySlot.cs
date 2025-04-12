namespace Dimworld.Core.Items;

using Godot;


/// <summary>
/// Represents a slot in an inventory.
/// This class is used to store an item and its quantity.
/// It also provides methods to add, remove, and swap items in the slot.
/// </summary>
[GlobalClass]
public partial class InventorySlot : Resource
{

    // public static InventorySlot From(InventorySlot inventorySlot)
    // {
    //     return new InventorySlot
    //     {
    //         _item = inventorySlot.Item?.Duplicate() as InventoryItem,
    //         _quantity = inventorySlot.Quantity
    //     };
    // }


    [Signal] public delegate void OnUpdatedEventHandler();

    [Export] public InventoryItem Item {
        get => _item;
        set {
            _item = value;
            OnUpdateItem();
            EmitSignal(SignalName.OnUpdated);
        }
    }
    private InventoryItem _item = null;

    [Export] public int Quantity {
        get => _quantity;
        set {
            _quantity = value;
            OnUpdateQuantity();
            EmitSignal(SignalName.OnUpdated);
        }
    }
    private int _quantity = 0;


    public bool IsEmpty => Item == null || Quantity == 0;
    public bool IsFull => Item != null && Item.MaxStackSize == Quantity;


    private void OnUpdateItem()
    {
        if (Item == null)
        {
            Quantity = 0;
        }
    }

    private void OnUpdateQuantity()
    {
        if (Item == null) return;
        _quantity = Mathf.Clamp(_quantity, 0, Item.MaxStackSize);
    }

    /// <summary>
    /// Add an item to the inventory slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A boolean indicating whether the item was added successfully.</returns>
    public bool AddItem(InventoryItem item)
    {
        if (IsFull) return false;

        Item = item;

        if (IsEmpty)
        {
            Quantity = 1;
        }
        else
        {
            Quantity++;
        }

        return true;
    }

    public bool CanAddItem(InventoryItem item)
    {
        if (IsFull) return false;

        if (Item == null)
        {
            return true;
        }

        if (Item != item)
        {
            return false;
        }

        if (Quantity >= Item.MaxStackSize)
        {
            return false;
        }

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

    /// <summary>
    /// Swaps the contents of this slot with the contents of another slot.
    /// </summary>
    /// <param name="slot">The slot to swap with.</param>
    /// <returns>True if the swap was successful, false otherwise.</returns>
    public bool SwapWithExisting(InventorySlot slot)
    {
        InventorySlot slotPrevious = Duplicate(true) as InventorySlot;

        _item = slot.Item; // ?.Duplicate() as InventoryItem;
        _quantity = slot.Quantity;

        slot.Item = slotPrevious.Item; //?.Duplicate() as InventoryItem;
        slot.Quantity = slotPrevious.Quantity;

        EmitSignal(SignalName.OnUpdated);
        slot.EmitSignal(SignalName.OnUpdated);

        return true;
    }

    /// <summary>
    /// Clears the contents of the inventory slot.
    /// </summary>
    public void ClearSlot()
    {
        _item = null;
        _quantity = 0;
        EmitSignal(SignalName.OnUpdated);
    }


    public override string ToString()
    {
        if (Item != null)
        {
            return $"{Item.ItemName} x{Quantity}";
        }

        return base.ToString();
    }

}
