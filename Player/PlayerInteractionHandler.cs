namespace Dimworld;

using Godot;


public partial class PlayerInteractionHandler : Node
{

    [Export] public Inventory PlayerInventory { get; set; }
    [Export] public AgentDetectionHandler AgentDetectionHandler { get; set; }
    [Export] public CursorFollower CursorFollower { get; set; }


    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_input"))
        {
            ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
            if (interactableObject == null) return;
            if (interactableObject is Node2D interactableObjectNode2D)
            {
                if (!AgentDetectionHandler.CanSee(interactableObjectNode2D)) return;
                interactableObject.InteractWith();
            }
        }
    }

}
