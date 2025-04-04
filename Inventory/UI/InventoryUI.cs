namespace Dimworld;

using Godot;


public partial class InventoryUI : Container
{

    [Signal] public delegate void OnVisibilityChangedEventHandler(bool visible);


    [Export] public Inventory TargetInventory {
        get => _targetInventory;
        set => SetTargetInventory(value);
    }
    private Inventory _targetInventory;
    [Export] public PackedScene SlotUIScene = GD.Load<PackedScene>("res://Inventory/UI/Slot/InventorySlotUI.tscn");

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


    public GridContainer SlotsGrid;


    private Label InventoryTitle;


    public override void _Ready()
    {
        InventoryTitle = GetNode<Label>("%InventoryTitle");
        SlotsGrid = GetNode<GridContainer>("%SlotsGrid");
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (_targetInventory == null) return;
        if (SlotsGrid == null) return;
        if (InventoryTitle == null) return;

        InventoryTitle.Text = _targetInventory.InventoryName;

        foreach (Node child in SlotsGrid.GetChildren())
        {
            child.QueueFree();
        }

        int index = 0;
        foreach (InventorySlot slot in _targetInventory.Slots)
        {
            InventorySlotUI slotUI = SlotUIScene.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            slotUI.ParentInventoryUI = this;
            SlotsGrid.AddChild(slotUI);

            int row = index / SlotsGrid.Columns;
            if (row >= RowsDisplayed)
            {
                slotUI.Hide();
            }

            index++;
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

    private void SetTargetInventory(Inventory inventory)
    {
        _targetInventory = inventory;
        UpdateUI();

        if (_targetInventory == null) return;

        _targetInventory.OnUpdated += UpdateUI;
    }

}
