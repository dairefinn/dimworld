namespace Dimworld;

using Godot;


public partial class PlayerController : Node
{

    [Export] public bool IsCurrentPlayer { get; set; } = false;
    // [Export] public Inventory Inventory { get; set; }
    [Export] public InventoryHandler InventoryHandler { get; set; }
    [Export] public CharacterController CharacterController { get; set; }
    // [Export] public DetectionHandler DetectionHandler { get; set; }
    [Export] public CursorFollower CursorFollower { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (IsCurrentPlayer && Globals.GetInstance().MainPlayer == null)
        {
            Globals.GetInstance().MainPlayer = this;
            InventoryHandler.PrimaryInventory = CharacterController.Inventory;
        }
    }

    public void TryInteract()
    {
        if (CursorFollower == null) return;

        ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
        if (interactableObject == null) return;

        if (interactableObject is Node2D interactableObjectNode2D)
        {
            if (!CharacterController.DetectionHandler.CanSee(interactableObjectNode2D)) return;
            interactableObject.InteractWith();
        }
    }

}
