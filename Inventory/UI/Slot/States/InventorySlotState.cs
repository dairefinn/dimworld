namespace Dimworld;

using Godot;

public partial class InventorySlotState : Node
{

    public enum State
    {
        BASE = 0,
        CLICKED = 1,
        DRAGGING = 2,
        ACTIVE = 3,
        RELEASED = 4
    }

    [Signal] public delegate void TransitionRequestedEventHandler(InventorySlotState from, State to);

    public virtual State StateId { get; set; }


    public InventorySlotUI inventorySlotUI;


    public virtual void Enter()
    {
        // Replace with function body
    }

    public virtual void PostEnter()
    {
        // Replace with function body
    }

    public virtual void Exit()
    {
        // Replace with function body
    }

    public virtual void OnInput(InputEvent @event)
    {
        // Replace with function body
    }

    public virtual void OnGuiInput(InputEvent @event)
    {
        // Replace with function body
    }

    public virtual void OnMouseEntered()
    {
        // Replace with function body
    }

    public virtual void OnMouseExited()
    {
        // Replace with function body
    }

}
