namespace Dimworld.Items.UI;

using Godot;
using Godot.Collections;


public partial class InventorySlotStateMachine : Node
{

	[Export] public InventorySlotState initialState;


	private InventorySlotState currentState;
	[Export] public Dictionary<InventorySlotState.State, InventorySlotState> states = [];


	public void Init(InventorySlotUI inventorySlot)
	{
		foreach (Node child in GetChildren())
		{
			if (child is InventorySlotState state)
			{
				states[state.StateId] = state;
				state.TransitionRequested += OnTransitionRequested;
				state.inventorySlotUI = inventorySlot;
			}
		}

		if (initialState != null)
		{
			initialState.Enter();
			currentState = initialState;
		}
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

	public void OnTransitionRequested(InventorySlotState from, InventorySlotState.State to)
	{
		if (Engine.IsEditorHint()) return; // Ignore transitions in the editor
		if (from != currentState) return; // Cannot transition from a state that is not the current state
		GD.Print($"Transition requested from {from} to {to}");

		InventorySlotState newState = states[to];

		if (newState == null) return; // Cannot transition to a state that does not exist

		if (currentState != null)
		{
			currentState.Exit();
		}

		newState.Enter();
		currentState = newState;
		newState.PostEnter();
	}

}
