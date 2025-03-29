namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class DamageOverTime : Area2D
{

    [Export] public int Damage = 10;
    [Export] public float Radius = 100f;
    [Export] public float Duration = 1f;
    [Export] public float DamageInterval = 1f;
    [Export] public bool Sticky = false; // If true, the damage will be applied to any body that enters the area, even if it leaves the area before the duration ends.


    private Array<Node> _bodies = [];


    public override void _Ready()
    {
        SetRadius(Radius);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

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
        (collisionShape.Shape as CircleShape2D).Radius = radius;
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
        if (Sticky) return; // If sticky, don't remove the body

        if (body is IDamageable damageable)
        {
            _bodies.Remove(body);
        }
    }

}
