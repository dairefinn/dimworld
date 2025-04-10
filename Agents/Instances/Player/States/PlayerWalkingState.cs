namespace Dimworld.Agents.Instances.States;

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

        if (Parent.DesiredMovementDirection == Vector2.Zero)
        {
            ParentStateMachine.TransitionTo(Player.States.Idle.ToString());
        }
        
    }

}
