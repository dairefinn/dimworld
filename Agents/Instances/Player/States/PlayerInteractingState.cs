namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
using Godot;


public partial class PlayerInteractingState : State<Player>
{

    public override string Key { get; set; } = "INTERACTING";

    public override void Enter()
    {
        base.Enter();

        TryInteract(Globals.Instance.CursorFollower);
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        Parent.DesiredMovementDirection = Vector2.Zero;

        if (!Globals.Instance.InventoryViewer.IsViewing)
        {
            ParentStateMachine.TransitionTo(Player.States.Idle.ToString());
            return;
        }
    }

    private void TryInteract(CursorFollower cursorFollower)
    {
        if (!IsInstanceValid(cursorFollower)) return;

        ICanBeInteractedWith interactableObject = cursorFollower.InteractableObject;
        if (interactableObject == null) return;

        Parent.TryInteractWith(interactableObject);
    }

}
