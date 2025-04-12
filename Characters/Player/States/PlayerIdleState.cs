namespace Dimworld.Agents.Instances.States;

using Dimworld.Characters;
using Dimworld.Core.StateMachines;
using Godot;


public partial class PlayerIdleState : State<Player>
{

    public override string Key { get; set; } = "IDLE";

    public override void Enter()
    {
        base.Enter();

        Parent.AnimationPlayer.Play("idle");
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        // If moving, transition to walking state
		if (Parent.DesiredMovementDirection != Vector2.Zero)
		{
			ParentStateMachine.TransitionTo(Player.States.Walking.ToString());
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
