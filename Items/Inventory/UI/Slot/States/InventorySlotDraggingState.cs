namespace Dimworld.Items.UI;

using Dimworld.States;
using Godot;


public partial class InventorySlotDraggingState : State<InventorySlotUI>
{

	public override string Key { get; set; } = "DRAGGING";

	// private bool _minimumDragTimeElapsed = false;
	private InventorySlotUI.StyleType? _previousStyleType;
	private Vector2 _dragAreaSize = Vector2.Zero;

    
    public override void Enter()
    {
		_previousStyleType = Parent.CurrentStyle;
		Parent.SetStyle(InventorySlotUI.StyleType.Selected);

		// _minimumDragTimeElapsed = false;
		// SceneTreeTimer thresholdTimer = Parent.GetTree().CreateTimer(DRAGGING_MINIMUM_THRESHOLD, false);
		// thresholdTimer.Timeout += () => _minimumDragTimeElapsed = true;

		Parent.DragArea.Monitoring = true;
		Parent.DragArea.Monitorable = false;

		_dragAreaSize = (Parent.DragArea.CollisionShape.Shape as RectangleShape2D).Size;
    }

	public override void Exit()
	{
		if (_previousStyleType != null)
		{
			Parent.SetStyle(_previousStyleType.Value);
		}
		else
		{
			Parent.SetStyle(InventorySlotUI.StyleType.Default);
		}
	}

    public override void OnInput(InputEvent @event)
    {

		if (@event is InputEventMouseMotion mouseMotion)
		{
			Parent.DragArea.GlobalPosition = mouseMotion.GlobalPosition - _dragAreaSize;
		}

		if (@event.IsActionPressed("rmb"))
		{
			ParentStateMachine.TransitionTo(this, InventorySlotUI.States.BASE.ToString());
			return;
		}
		
		if (@event.IsActionReleased("lmb"))
		{
			ReleaseDragOn(Parent.DragArea.Target);
			ParentStateMachine.TransitionTo(this, InventorySlotUI.States.BASE.ToString());
			return;
		}
    }

	private void ReleaseDragOn(InventorySlotDragArea target)
	{
		if (target == null) return;
		if (target.ParentSlot == null) return;
		if (target.ParentSlot == Parent) return;
		Globals.Instance.InventoryViewer.MoveItemFromSourceToDestination(Parent.ParentInventoryUI, target.ParentSlot.ParentInventoryUI, Parent, target.ParentSlot);
		// Globals.Instance.InventoryViewer.CallDeferred(InventoryViewer.MethodName.MoveItemFromSourceToDestination, [Parent.ParentInventoryUI, target.ParentSlot.ParentInventoryUI, Parent, target.ParentSlot]);
	}



}
