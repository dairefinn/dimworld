namespace Dimworld;

using Godot;


public partial class InputHandler : Node2D
{

    public override void _Process(double delta)
    {
        base._Process(delta);

        PlayerController mainPlayer = Globals.GetInstance().MainPlayer;

        bool canMove = !mainPlayer.InventoryViewer.IsViewing && Input.IsActionJustPressed("lmb");
        bool canAbortMove = Input.IsActionJustPressed("rmb");
        bool canToggleTimescale = Input.IsActionJustPressed("toggle_timescale");
        bool canInteract = Input.IsActionJustPressed("interact") && !mainPlayer.InventoryViewer.IsViewing;
        bool canOpenInventory = Input.IsActionJustPressed("toggle_inventory") && !mainPlayer.InventoryViewer.IsViewing;
        bool canCloseInventory = (Input.IsActionJustPressed("toggle_inventory") || Input.IsActionJustPressed("interact") || Input.IsActionJustPressed("ui_cancel")) && mainPlayer.InventoryViewer.IsViewing;

        if (canMove)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            mainPlayer.NavigateTo(mousePosition);
        }

        if (canAbortMove)
        {
            mainPlayer.StopNavigating();
        }

        if (canInteract)
        {
            mainPlayer.TryInteract();
        }

        if (canOpenInventory)
        {
            mainPlayer.InventoryViewer.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            mainPlayer.InventoryViewer.SetBothInventoriesVisibility(false);
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
