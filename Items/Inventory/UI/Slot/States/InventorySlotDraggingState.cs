namespace Dimworld.Items.UI;

using Godot;


public partial class InventorySlotDraggingState : InventorySlotState
{

	private static readonly float DRAGGING_MINIMUM_THRESHOLD = 0.05f;


    public override State StateId { get; set; } = State.DRAGGING;


	private bool _minimumDragTimeElapsed = false;
	private InventorySlotUI.StyleType? _previousStyleType;
	private Vector2 _dragAreaSize = Vector2.Zero;

    
    public override void Enter()
    {
		Node uiLayer = GetTree().GetFirstNodeInGroup("ui_layer");
		if (uiLayer != null)
		{
			inventorySlotUI.Reparent(uiLayer);
		}

		_previousStyleType = inventorySlotUI.CurrentStyle;
		inventorySlotUI.SetStyle(InventorySlotUI.StyleType.Selected);

		_minimumDragTimeElapsed = false;
		SceneTreeTimer thresholdTimer = GetTree().CreateTimer(DRAGGING_MINIMUM_THRESHOLD, false);
		thresholdTimer.Timeout += () => _minimumDragTimeElapsed = true;

		inventorySlotUI.DragArea.Monitoring = true;
		inventorySlotUI.DragArea.Monitorable = false;

		_dragAreaSize = (inventorySlotUI.DragArea.CollisionShape.Shape as RectangleShape2D).Size;
    }

	public override void Exit()
	{
		if (_previousStyleType != null)
		{
			inventorySlotUI.SetStyle(_previousStyleType.Value);
		}
		else
		{
			inventorySlotUI.SetStyle(InventorySlotUI.StyleType.Default);
		}
	}

    public override void OnInput(InputEvent @event)
    {

		if (@event is InputEventMouseMotion mouseMotion)
		{
			inventorySlotUI.DragArea.GlobalPosition = mouseMotion.GlobalPosition - _dragAreaSize;
		}

		if (@event.IsActionPressed("rmb"))
		{
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
			return;
		}
		
		if (@event.IsActionReleased("lmb") && _minimumDragTimeElapsed)
		{
			ReleaseDragOn(inventorySlotUI.DragArea.Target);
			EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
			return;
		}
    }

	private void ReleaseDragOn(InventorySlotDragArea target)
	{
		if (target == null) return;
		if (target.ParentSlot == null) return;
		if (target.ParentSlot == inventorySlotUI) return;
		Globals.Instance.InventoryViewer.MoveItemFromSourceToDestination(inventorySlotUI.ParentInventoryUI, target.ParentSlot.ParentInventoryUI, inventorySlotUI, target.ParentSlot);
		// Globals.Instance.InventoryViewer.CallDeferred(InventoryViewer.MethodName.MoveItemFromSourceToDestination, [inventorySlotUI.ParentInventoryUI, target.ParentSlot.ParentInventoryUI, inventorySlotUI, target.ParentSlot]);
	}



}
