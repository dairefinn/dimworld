namespace Dimworld;

using Godot;

public partial class InventoryUI : GridContainer
{

    [Export] public Inventory TargetInventory;


    public override void _Ready()
    {
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (TargetInventory == null) return;

        GD.Print("Updating UI");
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        foreach (InventorySlot slot in TargetInventory.Slots)
        {
            GD.Print("Adding slot UI");
            InventorySlotUI slotUI = new()
            {
                // TargetSlot = slot,
                // Size = new Vector2(20, 20)
            };
            AddChild(slotUI);
        }
    }

}
