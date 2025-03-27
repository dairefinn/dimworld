namespace Dimworld;

using Godot;


public partial class PlayerController : CharacterController
{

    [ExportGroup("Properties")]
    [Export] public bool IsCurrentPlayer { get; set; } = false;

    [ExportGroup("References")]
    [Export] public InventoryHandler InventoryViewer { get; set; }
    [Export] public CursorFollower CursorFollower { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (IsCurrentPlayer && Globals.GetInstance().MainPlayer == null)
        {
            Globals.GetInstance().MainPlayer = this;
            InventoryViewer.PrimaryInventory = Inventory;
        }

        SceneTreeTimer timer = GetTree().CreateTimer(3f);
        timer.Timeout += () => {
            GD.Print("PlayerController: Timeout");
            Stats.Health = Stats.MaxHealth / 3;
        };
    }

    public void TryInteract()
    {
        if (CursorFollower == null) return;

        ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
        if (interactableObject == null) return;


        if (interactableObject is Node2D interactableObjectNode2D)
        {
            if (!DetectionHandler.CanSee(interactableObjectNode2D)) return;

            interactableObject.InteractWith();
        }
    }

}
