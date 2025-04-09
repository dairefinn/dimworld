namespace Dimworld.Agents.Instances;

using Dimworld.Agents.Instances.States;
using Dimworld.Levels;
using Dimworld.States;
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

    public override void OnDeath()
    {
		_stateMachine.TransitionTo(States.Dead.ToString());
    }

}
