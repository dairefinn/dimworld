namespace Dimworld;

using Godot;


[Tool]
public partial class InventorySlotClickedState : InventorySlotState
{
	
	public override State StateId { get; set; } = State.CLICKED;

	public override void Enter()
	{
	}

    public override void OnInput(InputEvent @event)
    {
		// if (@event.IsActionReleased("lmb"))
		// {
		// 	EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
		// 	return;
		// }

        if (@event is InputEventMouse mouseEvent)
		{
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.DRAGGING);
			return;
		}
    }

}
