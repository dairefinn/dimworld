namespace Dimworld.UI.Inventory;

using Dimworld.Core.StateMachines;
using Godot;


public partial class InventorySlotClickedState : State<InventorySlotUI>
{

	public override string Key { get; set; } = "CLICKED";

	public override void Enter()
	{
	}

    public override void OnInput(InputEvent @event)
    {
        if (@event is InputEventMouse mouseEvent)
		{
			ParentStateMachine.TransitionTo(this, InventorySlotUI.States.DRAGGING.ToString());
			return;
		}
    }

}
