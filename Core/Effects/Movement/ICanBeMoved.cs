namespace Dimworld.Effects;

using Godot;


public interface ICanBeMoved
{
    
    public void ApplyVelocity(Vector2 velocity, double delta);

    public Vector2 GetGlobalPosition();

}
