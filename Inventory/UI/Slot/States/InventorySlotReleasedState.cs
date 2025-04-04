namespace Dimworld;

using Godot;


public partial class InventorySlotReleasedState : InventorySlotState
{

    public override State StateId { get; set; } = State.RELEASED;


    public override void Enter()
    {
        InventorySlotDragArea firstTarget = inventorySlotUI.DragArea.Targets.Count > 0 ? inventorySlotUI.DragArea.Targets[0] : null;
        if(firstTarget != null && inventorySlotUI != firstTarget.ParentSlot)
        {
            InputHandler.Instance.InventoryViewer.MoveItemFromSlotToSlot(inventorySlotUI, firstTarget.ParentSlot);
        }
    }

    public override void PostEnter()
    {
        EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
    }

}
