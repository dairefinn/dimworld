namespace Dimworld;

using Godot;
using Godot.Collections;


[Tool]
public partial class InventoryHotbar : Control
{

    [Export] public Inventory Inventory {
        get => _inventory;
        set {
            _inventory = value;
            OnUpdateInventory();
        }
    }
    private Inventory _inventory;


    [Export] public int ColumnCount {
        get => _columnCount;
        set {
            _columnCount = value;
            UpdateUI();
        }
    }
    private int _columnCount = 5;
    [Export] public int HotbarRow {
        get => _hotbarRow;
        set {
            _hotbarRow = value;
            UpdateUI();
        }
    }
    private int _hotbarRow = 0;

    [Export] private HBoxContainer SlotsContainer;


    public override void _Ready()
    {
        SlotsContainer = GetNode<HBoxContainer>("%SlotsContainer");

        UpdateUI();
    }


    public void OnUpdateInventory()
    {
        if (Inventory != null)
        {
            Inventory.OnUpdated += UpdateUI;
        }

        UpdateUI();
    }

    private Array<InventorySlot> GetInventorySlots()
    {
        Array<InventorySlot> slots = [];
        if (Inventory == null) return slots;

        for (int i = 0; i < Inventory.Slots.Count; i++)
        {
            int row = i / ColumnCount;
            if (row != HotbarRow) continue;
            InventorySlot slot = Inventory.Slots[i];
            slots.Add(slot);
        }

        return slots;
    }
    
    private void UpdateUI()
    {
        if (!IsInstanceValid(this)) return;

        Array<InventorySlot> slots = GetInventorySlots();

        if (IsInstanceValid(SlotsContainer))
        {
            // Remove existing children
            foreach (Node child in SlotsContainer.GetChildren())
            {
                if (!IsInstanceValid(child)) continue;

                if (child is InventorySlotUI slotUI)
                {
                    slotUI.QueueFree();
                }
            }

            // Add new slots
            int index = 1;
            foreach (InventorySlot slot in slots)
            {
                InventorySlotUI slotUI = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn").Instantiate<InventorySlotUI>();
                slotUI.TargetSlot = slot;
                slotUI.SlotIndex = index;
                slotUI.CanBeSelected = false;
                SlotsContainer?.AddChild(slotUI);
                index++;
            }
        }
    }

    public void SetSelectable(bool value)
    {
        if (!IsInstanceValid(this)) return;
        if (!IsInstanceValid(SlotsContainer)) return;

        foreach (Node child in SlotsContainer.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;

            if (child is InventorySlotUI slotUI)
            {
                if (!IsInstanceValid(slotUI)) continue;
                slotUI.CanBeSelected = value;
            }
        }
    }

}
