namespace Dimworld;

using Godot;


public partial class AgentMovementController : CharacterBody2D
{

    [ExportGroup("Properties")]
    [Export] public float Speed { get; set; } = 50f;
    [Export] public float Acceleration { get; set; } = 0.1f;

    [ExportGroup("References")]
    [Export] public NavigationAgent2D NavigationAgent { get; set; }


    // LIFECYCLE EVENTS

    public override void _Ready()
    {
        base._Ready();
        NavigationAgent.VelocityComputed += OnSafeVelocityComputed;
        // NavigationAgent.TargetReached += () => StopNavigating(); // TODO: Trying to hide the debug path after the target has been reached
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        ProcessNavigation(delta);
    }


    // NAVIGATION

    public void NavigateTo(Vector2 target)
    {
        if (NavigationAgent.TargetPosition == target) return;
        NavigationAgent.TargetPosition = target;
    }

    public void StopNavigating()
    {
        NavigationAgent.TargetPosition = GlobalPosition;
    }

    public bool CanReachPoint(Vector2 targetPoint)
    {
        NavigationAgent2D tempNavigationAgent = new();
        AddChild(tempNavigationAgent);
        tempNavigationAgent.TargetPosition = targetPoint;
        bool isTargetReachable = tempNavigationAgent.IsTargetReachable();
        tempNavigationAgent.QueueFree();
        return isTargetReachable;
    }

    private void ProcessNavigation(double delta)
    {
        if (NavigationAgent == null) return;
        if (NavigationAgent.IsNavigationFinished()) return;

        Vector2 nextPosition = NavigationAgent.GetNextPathPosition();
        Vector2 direction = GlobalPosition.DirectionTo(nextPosition);
        Vector2 newVelcity = direction * Speed;

        if (NavigationAgent.AvoidanceEnabled)
        {
            NavigationAgent.Velocity = newVelcity;
        }
        else
        {
            OnSafeVelocityComputed(newVelcity);
        }

        // Velocity = Velocity.Lerp(direction * Speed, (float)(Acceleration * delta));

        MoveAndSlide();
    }

    public void OnSafeVelocityComputed(Vector2 safeVelocity)
    {
        Velocity = safeVelocity;
    }

}
