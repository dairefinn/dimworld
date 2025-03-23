namespace Dimworld;

using Godot;


public partial class CursorFollower : Area2D
{

    public ICanBeInteractedWith InteractableObject;


    public override void _Ready()
    {
        base._Ready();

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        GlobalPosition = GetGlobalMousePosition();
    }


    public void OnBodyEntered(Node node)
    {
        if (node is ICanBeInteractedWith interactableObject)
        {
            InteractableObject = interactableObject;
        }
    }

    public void OnBodyExited(Node node)
    {
        if (node != InteractableObject) return;
        InteractableObject = null;
    }

}
