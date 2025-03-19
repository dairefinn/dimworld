namespace Dimworld;

using Godot;


public partial class InventoryUI : GridContainer
{

    [Export] public Inventory TargetInventory {
        get => _targetInventory;
        set
        {
            _targetInventory = value;
            value.OnUpdated += UpdateUI;
        }
    }
    private Inventory _targetInventory;
    [Export] public PackedScene SlotUIScene = GD.Load<PackedScene>("res://Inventory/UI/InventorySlotUI.tscn");


    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (TargetInventory == null) return;

        foreach (InventorySlot slot in TargetInventory.Slots)
        {
            InventorySlotUI slotUI = SlotUIScene.Instantiate<InventorySlotUI>();
            slotUI.TargetSlot = slot;
            AddChild(slotUI);
        }
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;

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
