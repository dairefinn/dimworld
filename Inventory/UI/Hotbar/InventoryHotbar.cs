namespace Dimworld;

using Godot;
using Godot.Collections;


[Tool]
public partial class InventoryHotbar : Control
{

    public static readonly PackedScene SCENE_INVENTORY_SLOT_UI = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn");

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
    [Export] public int SelectedSlotIndex {
        get => _selectedSlotIndex;
        set {
            _selectedSlotIndex = value;
            OnUpdateSelectedSlotIndex();
            UpdateUI();
        }
    }
    private int _selectedSlotIndex = -1;

    [ExportGroup("References")]
    [Export] public HBoxContainer SlotsContainer { get; set; }
    [Export] public PanelContainer SelectedBorder { get; set; }


    public InventorySlotUI SelectedSlotUI { get; set; }

    
    private Tween tweenSelectedBorder;


    public override void _Ready()
    {
        UpdateUI();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("hotbar_slot_prev"))
        {
            SelectPreviousSlot();
        }
        else if (@event.IsActionPressed("hotbar_slot_next"))
        {
            SelectNextSlot();
        }
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

        CallDeferred(MethodName.UpdateSlotUIs);
        CallDeferred(MethodName.UpdateSelectedBorderUI);
    }

    private void UpdateSlotUIs()
    {
        if (!IsInstanceValid(SlotsContainer)) return;

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
        Array<InventorySlot> slots = GetInventorySlots();
        foreach (InventorySlot slot in slots)
        {
            InventorySlotUI slotUI = SCENE_INVENTORY_SLOT_UI.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            slotUI.SlotIndex = index;
            slotUI.CanBeSelected = false;
            SlotsContainer?.AddChild(slotUI);
            index++;
        }
    }

    private void UpdateSelectedBorderUI()
    {
        if (!IsInstanceValid(SelectedBorder)) return;

        Array<InventorySlot> slots = GetInventorySlots();
        if (SelectedSlotIndex > -1 && SelectedSlotIndex < slots.Count)
        {
            SelectedBorder.Show();

            int slotWidth = 100;
            int slotGap = 10;
            int offsetX = (slotWidth + slotGap) * SelectedSlotIndex;

            tweenSelectedBorder?.Kill();
            tweenSelectedBorder = GetTree().CreateTween();
            tweenSelectedBorder.TweenProperty(SelectedBorder, "position", new Vector2(offsetX, SelectedBorder.Position.Y), 0.1f);
        }
        else
        {
            SelectedBorder.Hide();
            SelectedBorder.Position = new Vector2(0, SelectedBorder.Position.Y);
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

    private InventorySlotUI GetSelectedSlotUI()
    {
        if (!IsInstanceValid(this)) return null;
        if (!IsInstanceValid(SlotsContainer)) return null;

        foreach (Node child in SlotsContainer.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;

            if (child is InventorySlotUI slotUI)
            {
                if (slotUI.SlotIndex == (SelectedSlotIndex + 1))
                {
                    return slotUI;
                }
            }
        }

        return null;
    }

    private void SelectPreviousSlot()
    {
        if (!IsInstanceValid(this)) return;

        if (SelectedSlotIndex > 0)
        {
            SelectedSlotIndex--;
        }
        else
        {
            SelectedSlotIndex = GetInventorySlots().Count - 1;
        }
    }

    private void SelectNextSlot()
    {
        if (!IsInstanceValid(this)) return;

        if (SelectedSlotIndex < GetInventorySlots().Count - 1)
        {
            SelectedSlotIndex++;
        }
        else
        {
            SelectedSlotIndex = 0;
        }
    }

    private void OnUpdateSelectedSlotIndex()
    {
        if (!IsInstanceValid(this)) return;

        SelectedSlotUI = GetSelectedSlotUI();
    }

}
