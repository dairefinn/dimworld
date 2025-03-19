namespace Dimworld;

using Godot;


public partial class InventoryUI : Container
{

    [Signal] public delegate void OnVisibilityChangedEventHandler(bool visible);

    [Export] public Inventory TargetInventory {
        get => _targetInventory;
        set
        {
            _targetInventory = value;
            InventoryTitle.Text = value.InventoryName;
            value.OnUpdated += UpdateUI;
        }
    }
    private Inventory _targetInventory;
    [Export] public PackedScene SlotUIScene = GD.Load<PackedScene>("res://Inventory/UI/InventorySlotUI.tscn");


    private Label InventoryTitle;
    private GridContainer SlotsGrid;


    public override void _Ready()
    {
        InventoryTitle = GetNode<Label>("%InventoryTitle");
        SlotsGrid = GetNode<GridContainer>("%SlotsGrid");

        foreach (Node child in SlotsGrid.GetChildren())
        {
            child.QueueFree();
        }

        UpdateUI();
    }


    public void UpdateUI()
    {
        if (TargetInventory == null) return;

        InventoryTitle.Text = TargetInventory.InventoryName;

        foreach (InventorySlot slot in TargetInventory.Slots)
        {
            InventorySlotUI slotUI = SlotUIScene.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
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

        // This isn't really needed unless we're confining the mouse in some other way (e.g. if the player has to aim a gun)
        // if (Visible)
        // {
        //     Input.MouseMode = Input.MouseModeEnum.Confined;
        // }
        // else
        // {
        //     Input.MouseMode = Input.MouseModeEnum.Visible;
        // }
    }

}
