namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    public void InteractWith()
    {
        GD.Print("Interacting with chest");
    }

}
