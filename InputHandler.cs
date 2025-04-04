namespace Dimworld;

using Dimworld.Developer;
using Godot;


public partial class InputHandler : Node2D
{

    public override void _Process(double delta)
    {
        base._Process(delta);

        bool canUseInputs = !DeveloperMenu.IsOpen && !DeveloperConsole.IsFocused;

        bool isMovingManually = Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right");
        bool isMovingClick = Input.IsActionJustPressed("lmb");
        bool isTogglingDeveloperMenu = Input.IsActionJustPressed("toggle_developer_menu");

        bool canMove = canUseInputs && !Globals.Instance.InventoryViewer.IsViewing;
        bool canMoveClick = false;
        bool canAbortMove = canUseInputs && Input.IsActionJustPressed("rmb");
        bool canInteract = canUseInputs && Input.IsActionJustPressed("interact") && !Globals.Instance.InventoryViewer.IsViewing;
        bool canOpenInventory = canUseInputs && Input.IsActionJustPressed("toggle_inventory") && !Globals.Instance.InventoryViewer.IsViewing;
        bool canCloseInventory = canUseInputs && (Input.IsActionJustPressed("toggle_inventory") || Input.IsActionJustPressed("interact") || Input.IsActionJustPressed("ui_cancel")) && Globals.Instance.InventoryViewer.IsViewing;
        bool canCloseConsole = Input.IsActionJustPressed("ui_cancel") && DeveloperConsole.IsFocused;
        bool canUseHotbarItems = canUseInputs && !Globals.Instance.InventoryViewer.IsViewing && Input.IsActionJustPressed("lmb");
        

        if (canMove)
        {
            if (isMovingManually)
            {
                Vector2 direction = new(
                    Input.IsActionPressed("move_right") ? 1 : (Input.IsActionPressed("move_left") ? -1 : 0),
                    Input.IsActionPressed("move_down") ? 1 : (Input.IsActionPressed("move_up") ? -1 : 0)
                );

                Globals.Instance.Player.StopNavigating();
                Globals.Instance.Player.SetMovementDirection(direction);
            }
            else if (canMoveClick && isMovingClick)
            {
                Vector2 mousePosition = GetGlobalMousePosition();
                Globals.Instance.Player.NavigateTo(mousePosition);
            }
            else if (canAbortMove)
            {
                Globals.Instance.Player.StopNavigating();
            }
        }

        if (canInteract)
        {
            TryInteract();
        }

        if (canOpenInventory)
        {
            Globals.Instance.InventoryViewer.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            Globals.Instance.InventoryViewer.SetBothInventoriesVisibility(false);
        }

        if (isTogglingDeveloperMenu)
        {
            DeveloperMenu.Instance?.ToggleVisibility();
        }

        if (canCloseConsole)
        {
            DeveloperMenu.Instance?.Hide();
        }

        if (canUseHotbarItems)
        {
            Globals.Instance.InventoryViewer.TryUseSelectedItem();
        }

    }


    // INTERACTIONS

    public void TryInteract()
    {
        if (!IsInstanceValid(Globals.Instance.CursorFollower)) return;

        ICanBeInteractedWith interactableObject = Globals.Instance.CursorFollower.InteractableObject;
        if (interactableObject == null) return;

        Globals.Instance.Player.TryInteractWith(interactableObject);
    }
}
