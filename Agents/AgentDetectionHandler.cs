namespace Dimworld;

using Godot;
using Godot.Collections;
using System;
using System.Linq;


// TODO: Should only be able to "use" items within the interaction radius
// TODO: Should only be able to actually "see" items that are in line of sight (Cast a ray to check if there are obstacles in the way)
public partial class AgentDetectionHandler : Node2D
{

    [Signal] public delegate void OnNodeDetectedEventHandler(Node node);
    [Signal] public delegate void OnNodeLostEventHandler(Node node);


    [Export] public Area2D AreaVision { get; set; }
    [Export] public Array<Node> DetectedEntities { get; set; }
    [Export] public Label DebugTextOutput { get; set; }

    [Export] float InteractionRadius { get; set; } = 1;


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
            AreaVision.BodyEntered += OnNodeEnteredVision;
            AreaVision.BodyExited += OnNodeExitedVision;
        }
    }

    private void OnNodeEnteredVision(Node2D node)
    {
        DetectedEntities.Add(node);
        EmitSignal(SignalName.OnNodeDetected, node);
        UpdateDebugText();
    }

    private void OnNodeExitedVision(Node2D node)
    {
        DetectedEntities.Remove(node);
        EmitSignal(SignalName.OnNodeLost, node);
        UpdateDebugText();
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
