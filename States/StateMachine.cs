namespace Dimworld.States;

using Godot;
using Godot.Collections;


public class StateMachine<T>
{

	public Dictionary<string, State<T>> States = [];


	private State<T> currentState;


	public StateMachine()
	{

	}

	public StateMachine(T parent, Dictionary<string, State<T>> states, string initialState)
	{
		// Duplicate the states dictionary to avoid modifying the original resource
		States = states.Duplicate();

        // Iterate through all the states in the dictionary
        foreach (State<T> state in States.Values)
        {
			// Set the parent state machine and parent for each state
			state.ParentStateMachine = this;
            state.Parent = parent;
        }

		if (initialState != null)
		{
			EnterState(States[initialState]);
		}
	}

	
	private void EnterState(State<T> state)
	{
		currentState?.Exit();

		currentState = state;
		currentState.Enter();
		currentState.PostEnter();
	}


	public void OnInput(InputEvent _event)
	{
		currentState?.OnInput(_event);
	}

	public void OnGuiInput(InputEvent _event)
	{
		currentState?.OnGuiInput(_event);
	}

	public void OnMouseEntered()
	{
		currentState?.OnMouseEntered();
	}

	public void OnMouseExited()
	{
		currentState?.OnMouseExited();
	}

	public void TransitionTo(string to)
	{
		TransitionTo(currentState, to);
	}

	public void TransitionTo(State<T> from, string to)
	{
		if (Engine.IsEditorHint()) return; // Ignore transitions in the editor
		if (from != currentState) return; // Cannot transition from a state that is not the current state

		State<T> newState = States[to];

		if (newState == null) return; // Cannot transition to a state that does not exist

		EnterState(newState);
	}

}
