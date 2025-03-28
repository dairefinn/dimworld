namespace Dimworld;

using Godot;


public partial class Explosion : Area2D
{

    [Export] public int Damage = 10;
    [Export] public float Radius = 100f;
    [Export] public float Duration = 1f;


    private CollisionShape2D _collisionShape;
    private Timer _timer;


    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        (_collisionShape.Shape as CircleShape2D).Radius = Radius;

        BodyEntered += OnBodyEntered;

        _timer = new Timer();
        AddChild(_timer);
        _timer.WaitTime = Duration;
        _timer.OneShot = true;
        _timer.Timeout += () => QueueFree();
        _timer.Start();
    }

    private void OnBodyEntered(Node body)
    {
        if (body is IDamageable damageable)
        {
            damageable.TakeDamage(Damage);
        }
    }

}
