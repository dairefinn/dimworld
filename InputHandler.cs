namespace Dimworld;

using Godot;


public partial class InputHandler : Node2D
{

    [Export] public AgentMovementController PlayerAgent { get; set; }

    [Export] public Inventory PlayerInventory { get; set; }
    [Export] public InventoryUI InventoryUI { get; set; }

    [Export] public InventoryItem TempItem { get; set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("lmb"))
        {
            GD.Print("Setting target to mouse position: " + GetGlobalMousePosition());
            Vector2 mousePosition = GetGlobalMousePosition();
            PlayerAgent.NavigateTo(mousePosition);
        }

        if (Input.IsActionJustPressed("rmb"))
        {
            PlayerAgent.StopNavigating();
        }

        if (Input.IsActionJustPressed("toggle_inventory"))
        {
            InventoryUI.ToggleVisibility();
        }

        // TODO: Remove when done debugging
        if (Input.IsActionJustPressed("test_input"))
        {
            InventoryItem itemDuplicate = TempItem.Duplicate() as InventoryItem;
            bool success = PlayerInventory.AddItem(itemDuplicate);
            if (success)
            {
                GD.Print("Added item to inventory: " + itemDuplicate.ItemName);
            }
            else
            {
                GD.Print("Failed to add item to inventory: " + itemDuplicate.ItemName);
            }
        }
    }

}
