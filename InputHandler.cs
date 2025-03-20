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
        bool canToggleInventory = Input.IsActionJustPressed("toggle_inventory");
        bool canToggleTimescale = Input.IsActionJustPressed("toggle_timescale");
        bool canInteract = Input.IsActionJustPressed("interact");
        bool canCancel = Input.IsActionJustPressed("ui_cancel");

        if (canMove)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            mainPlayer.MovementController.NavigateTo(mousePosition);
        }

        if (canAbortMove)
        {
            mainPlayer.MovementController.StopNavigating();
        }

        if (canToggleInventory)
        {
            InventoryHandler.SetPrimaryInventoryVisibility(!InventoryHandler.GetPrimaryInventoryVisibility());
            InventoryHandler.CloseSecondaryInventory();
        }

        if (canInteract)
        {
            Globals.GetInstance().MainPlayer.TryInteract();
        }

        if (canCancel)
        {
            InventoryHandler.SetPrimaryInventoryVisibility(false);
            InventoryHandler.CloseSecondaryInventory();
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
