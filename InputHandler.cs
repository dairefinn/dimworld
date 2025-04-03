namespace Dimworld;

using Dimworld.Developer;
using Godot;


public partial class InputHandler : Node2D
{

    public static InputHandler Instance { get; private set; }


    [ExportGroup("References")]

    [Export] public CharacterController PlayerAgent {
        get => _playerAgent;
        set => SetPlayerAgent(value);
    }
    private CharacterController _playerAgent;

    [Export] public InventoryHandler InventoryViewer {
        get => _inventoryViewer;
        set => SetInventoryViewer(value);
    }
    private InventoryHandler _inventoryViewer;

    [Export] public CursorFollower CursorFollower { get; set; }


    public InputHandler()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("InputHandler: Attempted to create multiple instances of InputHandler.");
            QueueFree();
        }
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        bool canUseInputs = !DeveloperMenu.IsOpen && !DeveloperConsole.IsFocused;

        bool isMovingManually = Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right");
        bool isMovingClick = Input.IsActionJustPressed("lmb");
        bool isTogglingDeveloperMenu = Input.IsActionJustPressed("toggle_developer_menu");

        bool canMove = canUseInputs && !InventoryViewer.IsViewing;
        bool canAbortMove = canUseInputs && Input.IsActionJustPressed("rmb");
        bool canInteract = canUseInputs && Input.IsActionJustPressed("interact") && !InventoryViewer.IsViewing;
        bool canOpenInventory = canUseInputs && Input.IsActionJustPressed("toggle_inventory") && !InventoryViewer.IsViewing;
        bool canCloseInventory = canUseInputs && (Input.IsActionJustPressed("toggle_inventory") || Input.IsActionJustPressed("interact") || Input.IsActionJustPressed("ui_cancel")) && InventoryViewer.IsViewing;
        bool canCloseConsole = Input.IsActionJustPressed("ui_cancel") && DeveloperConsole.IsFocused;

        if (canMove)
        {
            if (isMovingManually)
            {
                Vector2 direction = new(
                    Input.IsActionPressed("move_right") ? 1 : (Input.IsActionPressed("move_left") ? -1 : 0),
                    Input.IsActionPressed("move_down") ? 1 : (Input.IsActionPressed("move_up") ? -1 : 0)
                );

                PlayerAgent.StopNavigating();
                PlayerAgent.SetMovementDirection(direction);
            }
            else if (isMovingClick)
            {
                Vector2 mousePosition = GetGlobalMousePosition();
                PlayerAgent.NavigateTo(mousePosition);
            }
            else if (canAbortMove)
            {
                PlayerAgent.StopNavigating();
            }
        }


        if (canInteract)
        {
            TryInteract();
        }

        if (canOpenInventory)
        {
            InventoryViewer.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            InventoryViewer.SetBothInventoriesVisibility(false);
        }

        if (isTogglingDeveloperMenu)
        {
            DeveloperMenu.Instance?.ToggleVisibility();
        }

        if (canCloseConsole)
        {
            DeveloperMenu.Instance?.Hide();
        }
    }


    // SETTERS

    public void SetPlayerAgent(CharacterController agent)
    {
        _playerAgent = agent;

        if (agent != null && InventoryViewer != null)
        {
            InventoryViewer.PrimaryInventory = agent.Inventory;
        }
    }

    public void SetInventoryViewer(InventoryHandler inventoryHandler)
    {
        _inventoryViewer = inventoryHandler;

        if (PlayerAgent != null && inventoryHandler != null)
        {
            inventoryHandler.PrimaryInventory = PlayerAgent.Inventory;
        }
    }


    // INTERACTIONS

    public void TryInteract()
    {
        if (CursorFollower == null) return;

        ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
        if (interactableObject == null) return;

        PlayerAgent.TryInteractWith(interactableObject);
    }
}
