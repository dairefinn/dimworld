namespace Dimworld;

using Godot;


public interface ICanBeMoved
{
    
    public void ApplyVelocity(Vector2 velocity);

    public Vector2 GetMoveableGlobalPosition();

}