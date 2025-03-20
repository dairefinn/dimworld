namespace Dimworld;

using Godot;


public partial class InputHandler : Node2D
{

    [Export] public InventoryHandler InventoryHandler { get; set; }


    public override void _Process(double delta)
    {
        base._Process(delta);

        PlayerController mainPlayer = Globals.GetInstance().MainPlayer;

        bool canMove = !mainPlayer.InventoryHandler.IsViewing && Input.IsActionJustPressed("lmb");
        bool canAbortMove = Input.IsActionJustPressed("rmb");
        bool canToggleTimescale = Input.IsActionJustPressed("toggle_timescale");
        bool canInteract = Input.IsActionJustPressed("interact") && !InventoryHandler.IsViewing;
        bool canOpenInventory = Input.IsActionJustPressed("toggle_inventory") && !InventoryHandler.IsViewing;
        bool canCloseInventory = (Input.IsActionJustPressed("toggle_inventory") || Input.IsActionJustPressed("interact") || Input.IsActionJustPressed("ui_cancel")) && InventoryHandler.IsViewing;

        if (canMove)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            mainPlayer.MovementController.NavigateTo(mousePosition);
        }

        if (canAbortMove)
        {
            mainPlayer.MovementController.StopNavigating();
        }

        if (canInteract)
        {
            Globals.GetInstance().MainPlayer.TryInteract();
        }

        if (canOpenInventory)
        {
            InventoryHandler.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            InventoryHandler.SetBothInventoriesVisibility(false);
        }
        
        // TODO: Lock behind debug menu
        if (canToggleTimescale)
        {
            if (Engine.TimeScale == 1.0)
            {
                Engine.TimeScale = 0.1f;
            }
            else
            {
                Engine.TimeScale = 1.0f;
            }
        }
    }

}
