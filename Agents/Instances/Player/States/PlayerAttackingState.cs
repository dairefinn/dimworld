namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
using Godot;

public partial class PlayerAttackingState : State<Player>
{

    public bool preventsMovement = true; // TODO: Let the weapon/Item decide this

    public override string Key { get; set; } = "ATTACKING";


    public override void Enter()
    {
        base.Enter();
        
        Globals.Instance.InventoryViewer.TryUseSelectedItem();
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);

        if (preventsMovement)
        {
            Parent.DesiredMovementDirection = Vector2.Zero;
        }

        if (!Parent.TryingToAttack)
        {
            ParentStateMachine.TransitionTo(Player.States.Idle.ToString());
        }
    }

}
