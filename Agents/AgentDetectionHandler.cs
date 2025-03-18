namespace Dimworld;

using Godot;
using Godot.Collections;
using System;
using System.Linq;


public partial class AgentDetectionHandler : Node2D
{
    
    // Sensory system properties
    [Export] public Area2D AreaVision { get; set; }
    [Export] public Area2D AreaInteraction { get; set; }
    [Export] public Array<Node> DetectedEntities { get; set; }
    [Export] public Label DebugTextOutput { get; set; }
    [Export] public AgentBrain AgentBrain { get; set; }

    private float InteractionRadius { get; set; } = 1;
    private float VisionRadius { get; set; } = 1;


    public override void _Ready()
    {
        DetectedEntities = [];
        base._Ready();
        Initialize();
    }


    private void Initialize()
    {
        if (AreaVision != null)
        {
            if (AreaVision.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                VisionRadius = circleShape.Radius;
            }

            AreaVision.BodyEntered += OnNodeEnteredVision;
            AreaVision.BodyExited += OnNodeExitedVision;
        }

        // TODO: This will be used to determine if an entity is close enough to "interact" with
        // It might just be better to check if you can see it and then if it's close enough distance wise
        // instead of requiring the CollisionShape.
        if (AreaInteraction != null)
        {
            if (AreaInteraction.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                InteractionRadius = circleShape.Radius;
            }
        }
    }

    private void OnNodeEnteredVision(Node2D node)
    {
        DetectedEntities.Add(node);
        UpdateDebugText();
        OnDetectedEntitiesChanged();
    }

    private void OnNodeExitedVision(Node2D node)
    {
        DetectedEntities.Remove(node);
        UpdateDebugText();
        OnDetectedEntitiesChanged();
    }

    private void OnDetectedEntitiesChanged()
    {
        bool allLightsEnabled = true;

        foreach (Node entity in DetectedEntities)
        {
            if (entity is LightBulb lightBulb)
            {
                if (!lightBulb.IsOn)
                {
                    allLightsEnabled = false;
                }
            }
        }

        GoapStateUtils.SetState(AgentBrain.WorldState, "all_visible_lights_on", allLightsEnabled);
    }

    // TODO: For debugging, remove after
    private void UpdateDebugText()
    {
        if (DebugTextOutput == null) return;

        if (DetectedEntities.Count() > 0)
        {
            DebugTextOutput.Text = "[" + string.Join(", ", DetectedEntities.Select(e => e.Name)) + "]";
        }
        else
        {
            DebugTextOutput.Text = "[]";
        }
    }

    public Array<T> GetDetectedInstancesOf<[MustBeVariant] T>() where T : Node
    {
        return [.. DetectedEntities.OfType<T>().ToArray()];
    }

    public T GetClosestDetectedInstanceOf<[MustBeVariant] T>() where T : Node2D
    {
        return GetClosestInstanceOf(GetDetectedInstancesOf<T>());
    }

    public T GetClosestDetectedInstanceOf<[MustBeVariant] T>(Array<T> options) where T : Node2D
    {
        Array<T> optionsDetected = GetDetectedInstancesOf<T>();
        if (optionsDetected.Count == 0) return null;

        Array<T> optionsDetectedAndAvailable = [..optionsDetected.Intersect(options).ToArray()];
        if (optionsDetectedAndAvailable.Count == 0) return null;

        return GetClosestInstanceOf(optionsDetectedAndAvailable);
    }

    public T GetClosestInstanceOf<[MustBeVariant] T>(Array<T> options) where T : Node2D
    {
        if (options == null || options.Count == 0) return null;

        Vector2 agentPosition = GlobalPosition;
        T closestItem = null;
        float closestDistance = float.MaxValue;

        foreach (T item in options)
        {
            float distance = agentPosition.DistanceTo(item.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

}
