namespace Dimworld.Agents.Instances.States;

using Dimworld.Characters;
using Dimworld.Core.StateMachines;
using Godot;


public partial class PlayerDeadState : State<Player>
{

    public override string Key { get; set; } = "DEAD";


    public override void Enter()
    {
        base.Enter();

        Parent.Rotate(Mathf.DegToRad(90));
        Parent.ClothingController.StopBlinking(false);
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        Parent.DesiredMovementDirection = Vector2.Zero;
    }

}
