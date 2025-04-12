namespace Dimworld.Agents.Instances.States;

using Dimworld.Core;
using Dimworld.Core.StateMachines;
using Dimworld.Core.Interaction;
using Godot;
using Dimworld.Characters;


public partial class PlayerAttackingState : State<Player>
{

    public bool preventsMovement = true; // TODO: Let the weapon/Item decide this

    public override string Key { get; set; } = "ATTACKING";


    private void OnAnimationFinished(StringName output)
    {
        Parent.ClothingController.FlipH = false;
    }


    public override void Enter()
    {
        base.Enter();
        
        Globals.Instance.InventoryViewer.TryUseSelectedItem();

        CursorFollower cursorFollower = Globals.Instance.CursorFollower;

        Parent.AnimationPlayer.AnimationFinished += OnAnimationFinished;

        if (cursorFollower.GlobalPosition.X < Parent.GlobalPosition.X)
        {
            Parent.AnimationPlayer.Play("attack_left");
        }
        else
        {
            Parent.AnimationPlayer.Play("attack");
        }
    }

    public override void Exit()
    {
        base.Exit();

        Parent.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
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
