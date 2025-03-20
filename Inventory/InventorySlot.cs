namespace Dimworld;

using Godot;

[GlobalClass]
public partial class InventorySlot : Resource
{

    [Signal] public delegate void OnUpdatedEventHandler();

    [Export] public InventoryItem Item {
        get => _item;
        set => SetItem(value);
    }
    private InventoryItem _item;
    [Export] public int Quantity {
        get => _quantity;
        set => SetQuantity(value);
    }
    private int _quantity = 0;

    public bool IsEmpty => Item == null || Quantity == 0;
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

    private void SetItem(InventoryItem value)
    {
        _item = value;

        if (_item == null)
        {
            _quantity = 0;
        }

        EmitSignal(SignalName.OnUpdated);
    }

    private void SetQuantity(int value)
    {
        if (Item == null)
        {
            _quantity = 0;
            return;
        }
        else
        {
            _quantity = Mathf.Clamp(value, 0, Item.MaxStackSize);
        }

        if (_quantity == 0)
        {
            _item = null;
        }

        EmitSignal(SignalName.OnUpdated);
    }

    public void AddFromExisting(InventorySlot slot)
    {
        if (slot == null) return;
        if (slot.IsEmpty) return;

        _item = slot.Item;
        _quantity = slot.Quantity;

        slot.ClearSlot();
        
        EmitSignal(SignalName.OnUpdated);
    }

    public void SwapWithExisting(InventorySlot slot)
    {
        if (slot == null) return;
        if (slot.IsEmpty) return;
        if (IsEmpty) return;

        InventorySlot slotPrevious = new()
        {
            Item = _item,
            Quantity = _quantity
        };

        _item = slot.Item;
        _quantity = slot.Quantity;

        slot.Item = slotPrevious.Item;
        slot.Quantity = slotPrevious.Quantity;

        EmitSignal(SignalName.OnUpdated);
    }

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
