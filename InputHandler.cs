namespace Dimworld;

using Dimworld.Developer;
using Dimworld.Items.UI;
using Godot;


public partial class InputHandler : Node2D
{

    public override void _Process(double delta)
    {
        base._Process(delta);

        InventoryViewer inventoryViewer = Globals.Instance.InventoryViewer;
        CharacterController player = Globals.Instance.Player;

        bool canUseInputs = !DeveloperMenu.IsOpen && !DeveloperConsole.IsFocused;

        bool isMovingManually = Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right");
        bool isMovingClick = Input.IsActionPressed("lmb");
        
        bool canMove = canUseInputs && !inventoryViewer.IsViewing;
        bool canMoveClick = false;
        bool canAbortMove = canUseInputs && Input.IsActionPressed("rmb");
        bool canSprint = canUseInputs && Input.IsActionPressed("action_sprint") && !inventoryViewer.IsViewing;
        
        if (canMove)
        {
            if (isMovingManually)
            {
                Vector2 direction = new(
                    Input.IsActionPressed("move_right") ? 1 : (Input.IsActionPressed("move_left") ? -1 : 0),
                    Input.IsActionPressed("move_down") ? 1 : (Input.IsActionPressed("move_up") ? -1 : 0)
                );

                player.StopNavigating();
                player.SetMovementDirection(direction);
            }
            else if (canMoveClick && isMovingClick)
            {
                Vector2 mousePosition = GetGlobalMousePosition();
                player.NavigateTo(mousePosition);
            }
            else if (canAbortMove)
            {
                player.StopNavigating();
            }

            if (canSprint)
            {
                player.SetSprinting(true);
            }
            else
            {
                player.SetSprinting(false);
            }
        }

    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        InventoryViewer inventoryViewer = Globals.Instance.InventoryViewer;
        CharacterController player = Globals.Instance.Player;
        CursorFollower cursorFollower = Globals.Instance.CursorFollower;

        bool canUseInputs = !DeveloperMenu.IsOpen && !DeveloperConsole.IsFocused;

        bool isTogglingDeveloperMenu = @event.IsActionPressed("toggle_developer_menu");

        bool canInteract = canUseInputs && @event.IsActionPressed("interact") && !inventoryViewer.IsViewing;
        bool canOpenInventory = canUseInputs && @event.IsActionPressed("toggle_inventory") && !inventoryViewer.IsViewing;
        bool canCloseInventory = canUseInputs && (@event.IsActionPressed("toggle_inventory") || @event.IsActionPressed("interact") || @event.IsActionPressed("ui_cancel")) && inventoryViewer.IsViewing;
        bool canUseHotbarItems = canUseInputs && !inventoryViewer.IsViewing && @event.IsActionPressed("lmb");
        bool canCloseConsole = @event.IsActionPressed("ui_cancel") && DeveloperConsole.IsFocused;
        

        if (canInteract)
        {
            TryInteract(player, cursorFollower);
        }

        if (canOpenInventory)
        {
            inventoryViewer.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            inventoryViewer.SetBothInventoriesVisibility(false);
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
            inventoryViewer.TryUseSelectedItem();
        }

        if (canUseInputs)
        {
            // If 1-5 are pressed, use the corresponding hotbar item
            if (@event.IsActionPressed("hotbar_slot_0"))
            {
                inventoryViewer.Hotbar.SelectSlot(0);
            }
            else if (@event.IsActionPressed("hotbar_slot_1"))
            {
                inventoryViewer.Hotbar.SelectSlot(1);
            }
            else if (@event.IsActionPressed("hotbar_slot_2"))
            {
                inventoryViewer.Hotbar.SelectSlot(2);
            }
            else if (@event.IsActionPressed("hotbar_slot_3"))
            {
                inventoryViewer.Hotbar.SelectSlot(3);
            }
            else if (@event.IsActionPressed("hotbar_slot_4"))
            {
                inventoryViewer.Hotbar.SelectSlot(4);
            }
        }
    }



    // INTERACTIONS

    public void TryInteract(CharacterController player, CursorFollower cursorFollower)
    {
        if (!IsInstanceValid(cursorFollower)) return;

        ICanBeInteractedWith interactableObject = cursorFollower.InteractableObject;
        if (interactableObject == null) return;

        player.TryInteractWith(interactableObject);
    }

}
