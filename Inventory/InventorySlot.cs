namespace Dimworld;

using Godot;


[GlobalClass]
public partial class InventorySlot : Resource
{

    [Signal] public delegate void OnUpdatedEventHandler();

    [Export] public InventoryItem Item {
        get => _item;
        set {
            _item = value;
            OnUpdate();
        }
    }
    private InventoryItem _item = null;
    [Export] public int Quantity {
        get => _quantity;
        set {
            _quantity = value;
            OnUpdate();
        }
    }
    private int _quantity = 0;

    public bool IsEmpty => Item == null || Quantity == 0;
    public bool IsFull => Item != null && Item.MaxStackSize == Quantity;


    public InventorySlot()
    {
    }

    public InventorySlot(InventorySlot inventorySlot)
    {
        Item = inventorySlot.Item;
        Quantity = inventorySlot.Quantity;
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

    private void OnUpdate()
    {
        if (Item == null)
        {
            _quantity = 0;
        }
        else
        {
            _quantity = Mathf.Clamp(_quantity, 0, Item.MaxStackSize);
        }

        EmitSignal(SignalName.OnUpdated);
    }

    public bool SwapWithExisting(InventorySlot slot)
    {
        InventorySlot slotPrevious = new()
        {
            Item = _item,
            Quantity = _quantity
        };

        _item = slot.Item?.Duplicate() as InventoryItem;
        _quantity = slot.Quantity;

        slot.Item = slotPrevious.Item?.Duplicate() as InventoryItem;
        slot.Quantity = slotPrevious.Quantity;

        EmitSignal(SignalName.OnUpdated);
        slot.EmitSignal(SignalName.OnUpdated);

        return true;
    }

    public void ClearSlot()
    {
        _item = null;
        OnUpdate();
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
