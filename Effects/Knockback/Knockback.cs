namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class Knockback : Area2D
{

    [Export] public float Radius = 100f;
    [Export] public float Force = 1000f;


    private Array<Node> _bodies = [];


    public override void _Ready()
    {
        SetRadius(Radius);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        GetTree().CreateTimer(0.1f).Timeout += () => Run();
    }


    private void Run()
    {
        MoveBodies();
        QueueFree();
    }

    private void MoveBodies()
    {
        foreach (Node body in _bodies)
        {
            if (body is ICanBeMoved moveable)
            {
                Vector2 direction = GlobalPosition.DirectionTo(moveable.GetMoveableGlobalPosition());
                Vector2 knockbackVelocity = direction * Force;
                moveable.ApplyVelocity(knockbackVelocity);
            }
        }
    }

    private void SetRadius(float radius)
    {
        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        (collisionShape.Shape as CircleShape2D).Radius = radius;
    }


    private void OnBodyEntered(Node body)
    {
        if (body is ICanBeMoved moveable)
        {
            _bodies.Add(body);
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is ICanBeMoved moveable)
        {
            _bodies.Remove(body);
        }
    }

}
