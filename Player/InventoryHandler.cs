namespace Dimworld;

using Godot;


public partial class InventoryHandler : Control
{

    [Export] public Inventory MainInventory;
    [Export] public Inventory SecondaryInventory;

    private InventoryUI primaryInventoryUI;
    private InventoryUI secondaryInventoryUI;

    public bool IsViewing => GetPrimaryInventoryVisibility() || GetSecondaryInventoryVisibility();


    public override void _Ready()
    {
        primaryInventoryUI = GetNode<InventoryUI>("%PrimaryInventoryUI");
        secondaryInventoryUI = GetNode<InventoryUI>("%SecondaryInventoryUI");

        primaryInventoryUI.TargetInventory = MainInventory;
        primaryInventoryUI.SetVisibility(false);
        secondaryInventoryUI.SetVisibility(false);
    }


    public void OpenSecondaryInventory(Inventory inventory)
    {
        SecondaryInventory = inventory;
        secondaryInventoryUI.TargetInventory = inventory;
        primaryInventoryUI.SetVisibility(true);
        secondaryInventoryUI.SetVisibility(true);
    }

    public void CloseSecondaryInventory()
    {
        SecondaryInventory = null;
        secondaryInventoryUI.TargetInventory = null;
        secondaryInventoryUI.SetVisibility(false);
    }

    public bool GetPrimaryInventoryVisibility()
    {
        return primaryInventoryUI.Visible;
    }

    public bool GetSecondaryInventoryVisibility()
    {
        return secondaryInventoryUI.Visible;
    }

    public void SetPrimaryInventoryVisibility(bool visible)
    {
        primaryInventoryUI.SetVisibility(visible);
    }

    public void SetSecondaryInventoryVisibility(bool visible)
    {
        secondaryInventoryUI.SetVisibility(visible);
    }

}
