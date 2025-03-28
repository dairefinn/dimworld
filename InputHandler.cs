namespace Dimworld;

using Godot;


public partial class InputHandler : Node2D
{

    public static InputHandler Instance { get; private set; }


    [ExportGroup("References")]
    [Export] public CharacterController PlayerAgent { get; set; }
    [Export] public InventoryHandler InventoryViewer { get; set; }
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

        bool canMove = !InventoryViewer.IsViewing && Input.IsActionJustPressed("lmb");
        bool canAbortMove = Input.IsActionJustPressed("rmb");
        bool canToggleTimescale = Input.IsActionJustPressed("toggle_timescale");
        bool canInteract = Input.IsActionJustPressed("interact") && !InventoryViewer.IsViewing;
        bool canOpenInventory = Input.IsActionJustPressed("toggle_inventory") && !InventoryViewer.IsViewing;
        bool canCloseInventory = (Input.IsActionJustPressed("toggle_inventory") || Input.IsActionJustPressed("interact") || Input.IsActionJustPressed("ui_cancel")) && InventoryViewer.IsViewing;

        if (canMove)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            PlayerAgent.NavigateTo(mousePosition);
        }

        if (canAbortMove)
        {
            PlayerAgent.StopNavigating();
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

    public void TryInteract()
    {
        if (CursorFollower == null) return;

        ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
        if (interactableObject == null) return;

        PlayerAgent.TryInteractWith(interactableObject);
    }
}
