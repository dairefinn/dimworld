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

		inventorySlotUI.Set("theme_override_styles/panel", InventorySlotUI.STYLEBOX_DEFAULT);

		// TODO: This will hide the tooltip when the state changes
		// Events.Instance.EmitSignal(Events.SignalName.TooltipHideRequested);
		
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
				// inventorySlotUI.RequestContextMenu() // TODO: Add context menu
			}

		}
	}

    public override void OnMouseEntered()
    {
		mouseOverInventorySlot = true;
		inventorySlotUI.Set("theme_override_styles/panel", InventorySlotUI.STYLEBOX_HOVER);
		// inventorySlotUI.RequestTooltip(); // TODO: Add tooltips
    }

	public override void OnMouseExited()
	{
		mouseOverInventorySlot = false;
		inventorySlotUI.Set("theme_override_styles/panel", InventorySlotUI.STYLEBOX_DEFAULT);
		// Events.Instance.EmitSignal(Events.SignalName.TooltipHideRequested);
	}

}
