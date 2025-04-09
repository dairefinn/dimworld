namespace Dimworld.States;

using Godot;


public partial class State<T> : Resource
{

    public virtual T Parent { get; set; }
    public virtual StateMachine<T> ParentStateMachine { get; set; }


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

    public virtual void OnProcess(double delta)
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
