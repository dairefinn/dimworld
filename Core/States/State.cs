namespace Dimworld.States;

using Godot;


public partial class State<T> : Resource
{

    public virtual string Key { get; set; } = string.Empty;
    public virtual T Parent { get; set; }
    public virtual StateMachine<T> ParentStateMachine { get; set; }


    public virtual void Enter() { }

    public virtual void PostEnter() { }

    public virtual void Exit() { }

    public virtual void OnProcess(double delta) { }

    public virtual void OnPhysicsProcess(double delta) { }

    public virtual void OnInput(InputEvent @event) { }

    public virtual void OnGuiInput(InputEvent @event) { }

    public virtual void OnMouseEntered() { }

    public virtual void OnMouseExited() { }

}
