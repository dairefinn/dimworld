namespace Dimworld;

using Godot;


public partial class InventorySlotClickedState : InventorySlotState
{
	
	public override State StateId { get; set; } = State.CLICKED;

	public override void Enter()
	{
        // TODO: Add drag/drop from one slot to another
		// inventorySlotUI.DropPointDetector.Monitoring = true;
	}

    public override void OnInput(InputEvent @event)
    {
        if (@event is InputEventMouse mouseEvent)
		{
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.DRAGGING);
			return;
		}
    }

}
