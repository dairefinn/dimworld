namespace Dimworld;

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
		
		inventorySlotUI.DragArea.Monitoring = false;
		inventorySlotUI.DragArea.Monitorable = true;
	}

	public override void OnGuiInput(InputEvent @event)
	{
		if (mouseOverInventorySlot)
		{
			if (@event.IsActionPressed("lmb"))
			{
				inventorySlotUI.DragArea.GlobalPosition = inventorySlotUI.DragArea.GetGlobalMousePosition();
				EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.CLICKED);
			}
			else if (@event.IsActionPressed("rmb"))
			{
				inventorySlotUI.ParentInventoryUI.ParentHandler.RequestContextMenu(inventorySlotUI);
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
