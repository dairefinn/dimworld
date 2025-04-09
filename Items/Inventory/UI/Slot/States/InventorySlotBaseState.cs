namespace Dimworld.Items.UI;

using Godot;


public partial class InventorySlotBaseState : InventorySlotState
{

	private bool mouseOverInventorySlot = false;

	public override State StateId { get; set; } = State.BASE;

    public override async void Enter()
	{
		if (!inventorySlotUI.IsNodeReady())
		{
			await ToSignal(inventorySlotUI, "ready");
		}
		
		inventorySlotUI.DragArea.Position = Vector2.Zero;

		inventorySlotUI.DragArea.Monitoring = false;
		inventorySlotUI.DragArea.Monitorable = true;
	}

	public override void OnGuiInput(InputEvent @event)
	{
		if (mouseOverInventorySlot && inventorySlotUI.CanBeSelected && !inventorySlotUI.TargetSlot.IsEmpty)
		{
			if (@event.IsActionPressed("shift_lmb"))
			{
				inventorySlotUI.EmitSignal(InventorySlotUI.SignalName.OnSlotClicked, inventorySlotUI);
				return;
			}
				
			if (@event.IsActionPressed("lmb"))
			{
				EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.CLICKED);
				return;
			}
			
			if (@event.IsActionPressed("rmb"))
			{
				Globals.Instance.InventoryViewer.RequestContextMenu(inventorySlotUI);
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
