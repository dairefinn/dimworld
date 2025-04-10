namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
using Godot;


public partial class PlayerRunningState : State<Player>
{

    public override string Key { get; set; } = "RUNNING";

    public override void Enter()
    {
        base.Enter();

        Parent.SetSprinting(true);
    }

    public override void Exit()
    {
        base.Exit();

        Parent.SetSprinting(false);
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        // If no longer sprinting or stopped moving, transition to walking state
        if (!Input.IsActionPressed(InputActions.ACTION_SPRINT) || Parent.DesiredMovementDirection == Vector2.Zero)
        {
            ParentStateMachine.TransitionTo(Player.States.Walking.ToString());
            return;
        }
    }

}
