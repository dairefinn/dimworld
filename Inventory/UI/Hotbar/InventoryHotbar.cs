namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class InventoryHotbar : Control
{

    [Export] public Inventory Inventory {
        get => _inventory;
        set {
            _inventory = value;
            UpdateUI();
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


    private HBoxContainer SlotsContainer;

    public override void _Ready()
    {
        SlotsContainer = GetNode<HBoxContainer>("%SlotsContainer");
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
        if (Inventory == null) return;
        if (SlotsContainer == null) return;

        // Remove existing children
        foreach (Node child in SlotsContainer.GetChildren())
        {
            if (child is InventorySlotUI slotUI)
            {
                slotUI.QueueFree();
            }
        }


        int index = 1;
        foreach (InventorySlot slot in GetInventorySlots())
        {
            InventorySlotUI slotUI = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn").Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            slotUI.SlotIndex = index;
            SlotsContainer.AddChild(slotUI);
            index++;
        }
    }

}
