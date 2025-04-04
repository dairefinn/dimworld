namespace Dimworld;

using System;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class InventoryUI : Container
{

    public static readonly PackedScene SCENE_SLOT_UI = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn");


    [Signal] public delegate void OnVisibilityChangedEventHandler(bool visible);


    [Export] public Inventory TargetInventory {
        get => _targetInventory;
        set {
            _targetInventory = value;
            OnUpdateTargetInventory();
        }
    }
    private Inventory _targetInventory;

    /// <summary>
    /// The number of rows to display. If the inventory has more than this, they will be hidden. This is used to hide the hotbar row from the full inventory view while keeping a single source of truth for the contents.
    /// </summary>
    [Export] public int RowsDisplayed {
        get => _rowsDisplayed;
        set {
            _rowsDisplayed = value;
            UpdateUI();
        }
    }
    private int _rowsDisplayed = 3;
    [Export] public int Columns {
        get => _columns;
        set {
            _columns = value;
            UpdateUI();
        }
    }
    private int _columns = 5;


    [ExportGroup("References")]
    [Export] public GridContainer SlotsGrid;
    [Export] public Label InventoryTitle;


    public override void _Ready()
    {
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (_targetInventory == null) return;

        if (IsInstanceValid(InventoryTitle))
        {
            InventoryTitle.Text = _targetInventory.InventoryName;
        }

        if (IsInstanceValid(SlotsGrid))
        {
            SlotsGrid.Columns = Columns;
        }

        if (SlotsGrid == null) return;

        // FIXME: We're re-creating every slot every UI update and this could probably be more efficient if we didn't do that.

        // Clear all previous slot UIs
        foreach (Node child in SlotsGrid.GetChildren())
        {
            if (child is InventorySlotUI slotUI)
            {
                slotUI.QueueFree();
            }
        }

        // Create new slot UIs
        for(int i = 0; i < _targetInventory.Slots.Count; i++)
        {
            InventorySlot currentSlot = _targetInventory.Slots[i];
            int row = i / SlotsGrid.Columns;
            if (row >= RowsDisplayed) return;
            InventorySlotUI slotUI = SCENE_SLOT_UI.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = currentSlot;
            slotUI.ParentInventoryUI = this;
            SlotsGrid.AddChild(slotUI);
        }
    }

    public void SetVisibility(bool isVisible)
    {
        Visible = isVisible;
        EmitSignal(SignalName.OnVisibilityChanged, isVisible);
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        EmitSignal(SignalName.OnVisibilityChanged, Visible);
    }

    private void OnUpdateTargetInventory()
    {
        UpdateUI();

        if (_targetInventory == null) return;
        _targetInventory.OnUpdated += UpdateUI;
    }

}
