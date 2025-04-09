namespace Dimworld.Items.UI;

using Godot;


public partial class InventorySlotDraggingState : InventorySlotState
{

    public override State StateId { get; set; } = State.DRAGGING;


	private const float DRAGGING_MINIMUM_THRESHOLD = 0.05f;
	private bool minimumDragTimeElapsed = false;
	private InventorySlotUI.StyleType? previousStyleType;

    
    public override void Enter()
    {
		Node uiLayer = GetTree().GetFirstNodeInGroup("ui_layer");
		if (uiLayer != null)
		{
			inventorySlotUI.Reparent(uiLayer);
		}

		previousStyleType = inventorySlotUI.GetStyle();
		inventorySlotUI.SetStyle(InventorySlotUI.StyleType.Selected);

		minimumDragTimeElapsed = false;
		SceneTreeTimer thresholdTimer = GetTree().CreateTimer(DRAGGING_MINIMUM_THRESHOLD, false);
		thresholdTimer.Timeout += () => minimumDragTimeElapsed = true;

		inventorySlotUI.DragArea.Monitoring = true;
		inventorySlotUI.DragArea.Monitorable = false;
    }

	public override void Exit()
	{
		if (previousStyleType != null)
		{
			inventorySlotUI.SetStyle(previousStyleType.Value);
		}
		else
		{
			inventorySlotUI.SetStyle(InventorySlotUI.StyleType.Default);
		}
	}

    public override void OnInput(InputEvent @event)
    {
		bool mouseMotion = @event is InputEventMouseMotion;
		bool cancel = @event.IsActionPressed("rmb");
		bool confirm = @event.IsActionPressed("lmb") || @event.IsActionReleased("lmb");

		if (mouseMotion)
		{
			inventorySlotUI.DragArea.GlobalPosition = inventorySlotUI.DragArea.GetGlobalMousePosition() - (inventorySlotUI.DragArea.CollisionShape.Shape as RectangleShape2D).Size;
		}

		if (cancel)
		{
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
			return;
		}
		
		if (minimumDragTimeElapsed && confirm)
		{
			GetViewport().SetInputAsHandled();
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.RELEASED);
			return;
		}
    }



}
