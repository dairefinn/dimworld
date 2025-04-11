namespace Dimworld.Items.UI;

using Godot;
using Godot.Collections;


public partial class InventoryHotbarUI : Control
{

    public static readonly PackedScene SCENE_INVENTORY_SLOT_UI = GD.Load<PackedScene>("res://Core/Items/Inventory/UI/Slot/InventorySlotUI.tscn");


    [Signal] public delegate void OnSlotSelectedEventHandler(InventorySlotUI selectedSlotUI);


    [Export] public Inventory Inventory {
        get => _inventory;
        set => SetInventory(value);
    }
    private Inventory _inventory;


    [Export] public int ColumnCount {
        get => _columnCount;
        set => SetColumnCount(value);
    }
    private int _columnCount = 5;
    [Export] public int HotbarRow {
        get => _hotbarRow;
        set => SetHotbarRow(value);
    }
    private int _hotbarRow = 0;
    [Export] public int SelectedSlotIndex {
        get => _selectedSlotIndex;
        set => SetSelectedSlotIndex(value);
    }
    private int _selectedSlotIndex = -1;


    [ExportGroup("References")]
    [Export] public HBoxContainer SlotsContainer { get; set; }
    [Export] public PanelContainer SelectedBorder { get; set; }


    public InventorySlotUI SelectedSlotUI { get; set; }

    
    private Tween tweenSelectedBorder;
    private bool _updateScheduled = true;


    public void SetInventory(Inventory value)
    {
        if (_inventory != null)
        {
            _inventory.OnUpdated -= OnInventoryUpdated;
        }

        _inventory = value;

        if (_inventory != null)
        {
            _inventory.OnUpdated += OnInventoryUpdated;
        }

        _updateScheduled = true;
    }

    public void SetColumnCount(int value)
    {
        _columnCount = value;
        _updateScheduled = true;
    }

    public void SetHotbarRow(int value)
    {
        _hotbarRow = value;
        _updateScheduled = true;
    }

    private void SetSelectedSlotIndex(int value)
    {
        _selectedSlotIndex = value;
        
        SelectedSlotUI = GetSelectedSlotUI();

        if (SelectedSlotUI != null)
        {
            EmitSignal(SignalName.OnSlotSelected, SelectedSlotUI);
        }

        UpdateSelectedBorderUI();
    }

    private void OnInventoryUpdated()
    {
        UpdateSelectedBorderUI();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_updateScheduled)
        {
            UpdateSlotUIs();
            _updateScheduled = false;
        }
    }


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_PREV))
        {
            SelectNextSlot();
        }
        else if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_NEXT))
        {
            SelectPreviousSlot();
        }
    }

    private Array<InventorySlot> GetInventorySlots(Array<InventorySlot> slots, int targetRow, int columns)
    {
        Array<InventorySlot> slotsOutput = [];
        if (Inventory == null) return slotsOutput;

        for (int i = 0; i < slots.Count; i++)
        {
            int row = i / columns;
            if (row != targetRow) continue;
            InventorySlot slot = slots[i];
            slotsOutput.Add(slot);
        }

        return slotsOutput;
    }

    private void UpdateSlotUIs()
    {
        if (!IsInstanceValid(SlotsContainer)) return;

        bool canAnySlotsBeSelected = false;

        // Remove existing children
        foreach (Node child in SlotsContainer.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;

            if (child is InventorySlotUI slotUI)
            {
                if (slotUI.CanBeSelected)
                {
                    canAnySlotsBeSelected = true;
                }
                slotUI.QueueFree();
            }
        }

        // Add new slots
        int index = 1;
        Array<InventorySlot> slots = GetInventorySlots(Inventory.Slots, HotbarRow, ColumnCount);
        foreach (InventorySlot slot in slots)
        {
            InventorySlotUI slotUI = SCENE_INVENTORY_SLOT_UI.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            slotUI.SlotIndex = index;
            slotUI.CanBeSelected = canAnySlotsBeSelected;
            SlotsContainer?.AddChild(slotUI);
            index++;
        }
    }

    private void UpdateSelectedBorderUI()
    {
        if (!IsInstanceValid(SelectedBorder)) return;

        Array<InventorySlot> inventorySlots = Inventory?.Slots ?? [];

        Array<InventorySlot> slots = GetInventorySlots(inventorySlots, HotbarRow, ColumnCount);
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
            if (child is not InventorySlotUI slotUI) continue;
            slotUI.CanBeSelected = value;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (!IsInstanceValid(this)) return;

        if (slotIndex < 0) return;
        if (slotIndex > GetInventorySlots(Inventory.Slots, HotbarRow, ColumnCount).Count) return;

        SelectedSlotIndex = slotIndex;
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
            SelectedSlotIndex = GetInventorySlots(Inventory.Slots, HotbarRow, ColumnCount).Count - 1;
        }
    }

    private void SelectNextSlot()
    {
        if (!IsInstanceValid(this)) return;

        if (SelectedSlotIndex < GetInventorySlots(Inventory.Slots, HotbarRow, ColumnCount).Count - 1)
        {
            SelectedSlotIndex++;
        }
        else
        {
            SelectedSlotIndex = 0;
        }
    }

}
