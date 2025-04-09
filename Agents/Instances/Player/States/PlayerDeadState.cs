namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
using Godot;


public partial class PlayerDeadState : State<Player>
{

    public override void Enter()
    {
        base.Enter();

        Parent.Rotate(Mathf.DegToRad(90));
        Parent.ClothingController.StopBlinking(false);
    }

}
