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

    [Export] public int ColumnCount { get; set; } = 5;
    [Export] public int HotbarRow { get; set; } = 0;


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


        foreach (InventorySlot slot in GetInventorySlots())
        {
            GD.Print($"Adding slot {slot}");
            InventorySlotUI slotUI = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn").Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            SlotsContainer.AddChild(slotUI);
        }
    }

}
