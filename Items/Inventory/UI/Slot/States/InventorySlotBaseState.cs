namespace Dimworld.Items.UI;

using Dimworld.States;
using Godot;


public partial class InventorySlotBaseState : State<InventorySlotUI>
{

	private bool mouseOverInventorySlot = false;


    public override async void Enter()
	{
		if (!Parent.IsNodeReady())
		{
			await ToSignal(Parent, "ready");
		}

		Parent.DragArea.Position = Vector2.Zero;

		Parent.DragArea.Monitoring = false;
		Parent.DragArea.Monitorable = true;
	}

	public override void OnGuiInput(InputEvent @event)
	{
		if (mouseOverInventorySlot && Parent.CanBeSelected)
		{
			if (@event.IsActionPressed("shift_lmb"))
			{
				Parent.EmitSignal(InventorySlotUI.SignalName.OnSlotClicked, Parent);
				return;
			}

			if (@event.IsActionPressed("lmb") && !Parent.TargetSlot.IsEmpty)
			{
				ParentStateMachine.TransitionTo(this, "CLICKED");
				return;
			}
			
			if (@event.IsActionPressed("rmb"))
			{
				Parent.EmitSignal(InventorySlotUI.SignalName.OnSlotAlternateClicked, Parent);
				return;
			}

		}
	}

    public override void OnMouseEntered()
    {
		mouseOverInventorySlot = true;
    }

	public override void OnMouseExited()
	{
		mouseOverInventorySlot = false;
	}

}
