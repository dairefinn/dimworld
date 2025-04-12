namespace Dimworld.Core.Interaction;

using Godot;


/// <summary>
/// A class that follows the mouse cursor and detects when it enters or exits an interactable object.
/// </summary>
[GlobalClass]
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
        if (node is not ICanBeInteractedWith interactableObject) return;
        InteractableObject = interactableObject;
    }

    public void OnBodyExited(Node node)
    {
        if (node != InteractableObject) return;
        InteractableObject = null;
    }

}
