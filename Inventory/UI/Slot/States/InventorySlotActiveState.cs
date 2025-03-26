namespace Dimworld;

using Godot;


public partial class InventorySlotActiveState : InventorySlotState
{

    public override State StateId { get; set; } = State.ACTIVE;


    public override void Enter()
    {
    }

	public override void Exit()
	{
	}

}
