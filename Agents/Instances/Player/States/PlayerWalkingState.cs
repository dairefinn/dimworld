namespace Dimworld.Agents.Instances.States;

using Dimworld.Items;
using Dimworld.States;
using Godot;


public partial class PlayerWalkingState : State<Player>
{

    public override string Key { get; set; } = "WALKING";

    public override void Enter()
    {
        base.Enter();

        Parent.AnimationPlayer.Play("walk");
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        // If no longer moving, transition to idle state
        if (Parent.DesiredMovementDirection == Vector2.Zero)
        {
            ParentStateMachine.TransitionTo(Player.States.Idle.ToString());
            return;
        }

        // If sprinting, transition to sprinting state
        if (Input.IsActionPressed(InputActions.ACTION_SPRINT))
        {
            ParentStateMachine.TransitionTo(Player.States.Running.ToString());
            return;
        }

        // If attacking, transition to attacking state
        if (Parent.TryingToAttack)
        {
            ParentStateMachine.TransitionTo(Player.States.Attacking.ToString());
            return;
        }

        // If interacting, transition to interacting state
        if (Parent.TryingToInteract)
        {
            ParentStateMachine.TransitionTo(Player.States.Interacting.ToString());
            return;
        }
    }

}
