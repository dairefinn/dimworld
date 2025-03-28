namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class Fire : Area2D
{

    [Export] public int Damage = 10;
    [Export] public float Radius = 100f;
    [Export] public float Duration = 1f;
    [Export] public float DamageInterval = 1f;


    private Array<Node> _bodies = [];


    public override void _Ready()
    {
        SetRadius(Radius);

        BodyEntered += OnBodyEntered;

        CreateTimeout(Duration);

        StartDamageTimer();
    }

    private void StartDamageTimer()
    {
        SceneTreeTimer timer = GetTree().CreateTimer(DamageInterval);
        timer.Timeout += () => {
            DamageBodies();
            StartDamageTimer();
        };
    }

    private void DamageBodies()
    {
        foreach (Node body in _bodies)
        {
            if (body is IDamageable damageable)
            {
                damageable.TakeDamage(Damage);
            }
        }
    }

    private void SetRadius(float radius)
    {
        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        (collisionShape.Shape as CircleShape2D).Radius = Radius;
    }

    private void CreateTimeout(float duration)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(duration);
        timer.Timeout += () => QueueFree();
    }


    private void OnBodyEntered(Node body)
    {
        if (body is IDamageable damageable)
        {
            _bodies.Add(body);
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is IDamageable damageable)
        {
            _bodies.Remove(body);
        }
    }

}
