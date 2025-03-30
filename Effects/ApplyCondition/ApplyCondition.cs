namespace Dimworld;

using Godot;


public partial class ApplyCondition : Area2D
{

    [Export] public Condition Condition = null;
    [Export] public float Radius = 100f;
    [Export] public float Duration = 1f;


    public override void _Ready()
    {
        SetRadius(Radius);

        BodyEntered += OnBodyEntered;

        CreateTimeout(Duration);
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
        if (body is not IAffectedByConditions target) return;
        Condition.ApplyTo(target);
    }

}
