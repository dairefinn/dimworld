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


    private Label InventoryTitle;
    private GridContainer SlotsGrid;


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

        foreach (InventorySlot slot in _targetInventory.Slots)
        {
            InventorySlotUI slotUI = SlotUIScene.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            slotUI.ParentInventory = this;
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

    private void SetTargetInventory(Inventory inventory)
    {
        _targetInventory = inventory;
        UpdateUI();

        if (_targetInventory == null) return;

        _targetInventory.OnUpdated += UpdateUI;
    }

}
