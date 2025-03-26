namespace Dimworld;

using Godot;


public partial class InventorySlotReleasedState : InventorySlotState
{

    public override State StateId { get; set; } = State.RELEASED;


    public override void Enter()
    {
        InventorySlotDragArea firstTarget = inventorySlotUI.DragArea.Targets.Count > 0 ? inventorySlotUI.DragArea.Targets[0] : null;
        if(firstTarget != null)
        {
            MoveItemFromSlotToSlot(inventorySlotUI, firstTarget.ParentSlot);
        }
    }

    public override void PostEnter()
    {
        EmitSignal(InventorySlotState.SignalName.TransitionRequested, this, (int)State.BASE);
    }

    private void MoveItemFromSlotToSlot(InventorySlotUI sourceSlot, InventorySlotUI targetSlot)
    {
        if (sourceSlot == null) return;
        if (targetSlot == null) return;
        if (sourceSlot.TargetSlot.IsEmpty) return;
        if (sourceSlot == targetSlot) return;
        
        bool isChangingInventories = sourceSlot.ParentInventory != targetSlot.ParentInventory;
        targetSlot.TargetSlot.SwapWithExisting(sourceSlot.TargetSlot);

        // TODO:
        // if (isChangingInventories)
        // {
        //     PrimaryEquipmentHandler.Unequip(sourceSlot.TargetSlot.Item);
        //     PrimaryEquipmentHandler.Unequip(targetSlot.TargetSlot.Item);
        // }
    }

}
