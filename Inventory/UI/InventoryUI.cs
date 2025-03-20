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
    [Export] public PackedScene SlotUIScene = GD.Load<PackedScene>("res://Inventory/UI/InventorySlotUI.tscn");
    [Export] public int SelectedSlotIndex = -1; // TODO: Should I store the index or the slot here?


    private Label InventoryTitle;
    private GridContainer SlotsGrid;


    public override void _Ready()
    {
        InventoryTitle = GetNode<Label>("%InventoryTitle");
        SlotsGrid = GetNode<GridContainer>("%SlotsGrid");
        OnVisibilityChanged += OnVisibilityChangedInner;
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
            SlotsGrid.AddChild(slotUI);
            slotUI.OnClicked += () => OnClickSlot(slotUI);
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

    private void OnClickSlot(InventorySlotUI slotUI)
    {
        GD.Print("Slot clicked: " + slotUI);
        slotUI.IsSelected = true;
        int slotIndex = _targetInventory.Slots.IndexOf(slotUI.TargetSlot);
        SelectedSlotIndex = slotIndex;
    }

    private void OnVisibilityChangedInner(bool isVisible)
    {
        if (!isVisible)
        {
            SelectedSlotIndex = -1;
        }
    }

}
