namespace Dimworld;

using Godot;


public partial class Projectile : Node2D
{

    [Export] public int Speed = 1000;
    [Export] public float Lifetime = 5f;
    [Export] public Vector2 Direction = Vector2.Zero;

    private Vector2 _velocity;
    private float _lifetimeTimer = 0f;

    public override void _Ready()
    {
        // Initialize the velocity based on the direction of the projectile
        _velocity = new Vector2(1, 0).Normalized() * Speed; // Example direction
    }

    public override void _Process(double delta)
    {
        // Move the projectile
        Position += _velocity * (float)delta;

        // Update lifetime timer
        _lifetimeTimer += (float)delta;
        if (_lifetimeTimer >= Lifetime)
        {
            QueueFree(); // Remove the projectile after its lifetime expires
        }
    }

}