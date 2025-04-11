namespace Dimworld.Items.UI;

using Godot;
using Godot.Collections;


public partial class InventoryUI : Container
{

    public static readonly PackedScene SCENE_SLOT_UI = GD.Load<PackedScene>("res://Core/Items/Inventory/UI/Slot/InventorySlotUI.tscn");


    [Signal] public delegate void OnVisibilityChangedEventHandler(bool visible);
    [Signal] public delegate void OnSlotClickedEventHandler(InventorySlotUI slotUI);
    [Signal] public delegate void OnSlotAlternateClickedEventHandler(InventorySlotUI slotUI);


    /// <summary>
    /// The inventory this UI is displaying. This is the source of truth for the contents of the inventory.
    /// </summary>
    [Export] public Inventory TargetInventory {
        get => _targetInventory;
        set => SetTargetInventory(value);
    }
    private Inventory _targetInventory;

    /// <summary>
    /// The number of rows to display. If the inventory has more than this, they will be hidden. This is used to hide the hotbar row from the full inventory view while keeping a single source of truth for the contents.
    /// </summary>
    [Export] public int RowsDisplayed {
        get => _rowsDisplayed;
        set {
            _rowsDisplayed = value;
            UpdateSlotsGrid(TargetInventory.Slots);
        }
    }
    private int _rowsDisplayed = 3;

    /// <summary>
    /// The number of columns to display. This is entirely for UI purposes and does not affect the inventory itself as the inventory is a 1D array with no concept of rows or columns.
    /// </summary>
    [Export] public int Columns {
        get => _columns;
        set {
            _columns = value;
            UpdateSlotsGridColumns(value);
        }
    }
    private int _columns = 5;


    [ExportGroup("References")]
    [Export] public GridContainer SlotsGrid;
    [Export] public Label InventoryTitle;


    private void UpdateTitleLabel(string value)
    {
        if (!IsInstanceValid(InventoryTitle)) return;
        InventoryTitle.Text = value;
    }

    private void UpdateSlotsGrid(Array<InventorySlot> slots)
    {
        if (!IsInstanceValid(SlotsGrid)) return;

        // Clear all previous slot UIs
        foreach (Node child in SlotsGrid.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;

            if (child is InventorySlotUI slotUI)
            {
                slotUI.QueueFree();
            }
        }

        // Create new slot UIs
        for(int i = 0; i < slots.Count; i++)
        {
            InventorySlot currentSlot = slots[i];
            int row = i / SlotsGrid.Columns;
            if (row >= RowsDisplayed) return;
            InventorySlotUI slotUI = SCENE_SLOT_UI.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = currentSlot;
            slotUI.ParentInventoryUI = this;
            slotUI.OnSlotClicked += OnSlotClickedInner;
            slotUI.OnSlotAlternateClicked += OnSlotAlternateClickedInner;
            SlotsGrid?.AddChild(slotUI);
        }
    }

    private void UpdateSlotsGridColumns(int columns)
    {
        if (!IsInstanceValid(SlotsGrid)) return;

        SlotsGrid.Columns = columns;
    }

    private void OnSlotClickedInner(InventorySlotUI slotUI)
    {
        EmitSignal(SignalName.OnSlotClicked, slotUI);
    }

    private void OnSlotAlternateClickedInner(InventorySlotUI slotUI)
    {
        EmitSignal(SignalName.OnSlotAlternateClicked, slotUI);
    }

    
    /// <summary>
    /// Sets the target inventory for this UI. This will update the title label and the slots grid to match the new inventory.
    /// </summary>
    /// <param name="value">The inventory to set as the target</param>
    public void SetTargetInventory(Inventory value)
    {
        _targetInventory = value;

        if (_targetInventory != null)
        {
            UpdateTitleLabel(value.InventoryName);
            UpdateSlotsGrid(value.Slots);
        }
    }

    /// <summary>
    /// Sets the visibility of the inventory UI
    /// </summary>
    /// <param name="isVisible">True to show the inventory, false to hide it</param>
    public void SetVisibility(bool isVisible)
    {
        Visible = isVisible;
        EmitSignal(SignalName.OnVisibilityChanged, isVisible);
    }

    /// <summary>
    /// Toggles the visibility of the inventory UI
    /// </summary>
    public void ToggleVisibility()
    {
        Visible = !Visible;
        EmitSignal(SignalName.OnVisibilityChanged, Visible);
    }

    /// <summary>
    /// Returns the first empty slot in the inventory
    /// </summary>
    /// <returns>The first empty slot in the inventory, or null if there are no empty slots</returns>
    public InventorySlotUI GetFirstEmptySlot()
    {
        if (TargetInventory == null) return null;

        InventorySlot firstEmptySlot = TargetInventory.GetFirstEmptySlot();
        if (firstEmptySlot == null) return null;

        InventorySlotUI firstEmptySlotUI = GetSlotUIForSlot(firstEmptySlot);
        if (firstEmptySlotUI == null) return null;

        return firstEmptySlotUI;
    }

    /// <summary>
    /// Finds a slot in the UI which is linked to a given inventory slot.
    /// </summary>
    /// <param name="slot">The inventory slot to find the UI for</param>
    /// <returns>The InventorySlotUI linked to the given inventory slot, or null if not found</returns>
    public InventorySlotUI GetSlotUIForSlot(InventorySlot slot)
    {
        if (slot == null) return null;

        foreach (Node child in SlotsGrid.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;
            if (child is not InventorySlotUI slotUI) continue;

            if (slotUI.TargetSlot == slot)
            {
                return slotUI;
            }
        }

        return null;
    }

}
