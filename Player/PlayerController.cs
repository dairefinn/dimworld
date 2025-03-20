namespace Dimworld;

using Godot;


public partial class PlayerController : Node
{

    [Export] public bool IsCurrentPlayer { get; set; } = false;
    [Export] public Inventory Inventory { get; set; }
    [Export] public InventoryHandler InventoryHandler { get; set; }
    [Export] public AgentMovementController MovementController { get; set; }
    [Export] public AgentDetectionHandler AgentDetectionHandler { get; set; }
    [Export] public CursorFollower CursorFollower { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (IsCurrentPlayer && Globals.GetInstance().MainPlayer == null)
        {
            Globals.GetInstance().MainPlayer = this;
            InventoryHandler.PrimaryInventory = Inventory;
        }
    }

    public void TryInteract()
    {
        if (CursorFollower == null) return;

        ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
        if (interactableObject == null) return;

        if (interactableObject is Node2D interactableObjectNode2D)
        {
            if (!AgentDetectionHandler.CanSee(interactableObjectNode2D)) return;
            interactableObject.InteractWith();
        }
    }

}
