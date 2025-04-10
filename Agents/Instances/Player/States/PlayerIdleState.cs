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

}
