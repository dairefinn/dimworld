namespace Dimworld.Agents.Instances;

using Dimworld.Agents.Instances.States;
using Dimworld.Levels;
using Dimworld.States;
using Godot;
using Godot.Collections;


public partial class Player : CharacterController, ICanTriggerLevelTransitions
{

	private StateMachine<Player> _stateMachine;
	private Dictionary<string, State<Player>> _states = new()
	{
		{ States.Idle.ToString(), new PlayerIdleState() },
		{ States.Walking.ToString(), new PlayerWalkingState() },
		{ States.Running.ToString(), new PlayerRunningState() },
		{ States.Attacking.ToString(), new PlayerAttackingState() },
		{ States.Interacting.ToString(), new PlayerInteractingState() },
		{ States.Dead.ToString(), new PlayerDeadState() }
	};


    public Player()
    {
        Globals.Instance.Player = this;
    }


	public override void _Ready()
	{
		base._Ready();

        _stateMachine = new StateMachine<Player>(this, _states, States.Idle.ToString());
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		_stateMachine?.OnProcess(delta);
    }

    public override void OnDeath()
    {
		_stateMachine.TransitionTo(States.Dead.ToString());
    }

	public override void SetDesiredMovementDirection(Vector2 direction)
	{
		if (direction == Vector2.Zero) return;
		DesiredMovementDirection = direction.Normalized();

		_stateMachine.TransitionTo(States.Walking.ToString());
	}

}
