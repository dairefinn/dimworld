namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class PushPull : Area2D
{

    [Export] public float Radius = 100f;
    [Export] public float Force = 1000f;
    [Export] public float Duration = 0.1f;
    [Export] public DirectionType Direction = DirectionType.PUSH;


    private Array<Node> _bodies = [];


    public override void _Ready()
    {
        SetRadius(Radius);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        CreateTimeout(Duration);
    }

    public override void _Process(double delta)
    {
        MoveBodies(delta);
    }


    private void MoveBodies(double delta)
    {
        foreach (Node body in _bodies)
        {
            if (body is ICanBeMoved moveable)
            {
                Vector2 pushDirection = GlobalPosition.DirectionTo(moveable.GetMoveableGlobalPosition());
                if (Direction == DirectionType.PULL)
                {
                    pushDirection = pushDirection.Rotated(Mathf.DegToRad(180));
                }
                Vector2 knockbackVelocity = pushDirection * Force;
                moveable.ApplyVelocity(knockbackVelocity, delta);
            }
        }
    }

    private void SetRadius(float radius)
    {
        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        (collisionShape.Shape as CircleShape2D).Radius = radius;
    }

    private void CreateTimeout(float duration)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(duration);
        timer.Timeout += () => QueueFree();
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

    public enum DirectionType
    {
        PUSH,
        PULL
    }

}
