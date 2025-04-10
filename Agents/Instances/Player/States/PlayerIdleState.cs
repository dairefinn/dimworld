namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
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

		bool isMoving = Parent.DesiredMovementDirection != Vector2.Zero; // TODO: Transition to walking state
		if (isMoving)
		{
			ParentStateMachine.TransitionTo(CharacterController.States.Walking.ToString());
		}
    }

}
