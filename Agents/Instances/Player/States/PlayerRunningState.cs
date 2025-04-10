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

        if (!Input.IsActionPressed(InputActions.ACTION_SPRINT))
        {
            ParentStateMachine.TransitionTo(CharacterController.States.Walking.ToString());
        }
    }

}
