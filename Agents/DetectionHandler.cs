namespace Dimworld;

using Godot;
using Godot.Collections;
using System;
using System.Linq;


// TODO: Should only be able to actually "see" items that are in line of sight (Cast a ray to check if there are obstacles in the way)
public partial class DetectionHandler : Area2D
{

    [Signal] public delegate void DebugOutputEventHandler(string message);
    [Signal] public delegate void OnNodeDetectedEventHandler(Node node);
    [Signal] public delegate void OnNodeLostEventHandler(Node node);

    [Export] public Array<Node> DetectedEntities { get; set; }


    public override void _Ready()
    {
        if (DetectedEntities == null)
        {
            DetectedEntities = [];
        }
        base._Ready();
        Initialize();
    }


    private void Initialize()
    {
        BodyEntered += OnNodeEnteredVision;
        BodyExited += OnNodeExitedVision;
    }

    private void OnNodeEnteredVision(Node2D node)
    {
        DetectedEntities.Add(node);
        EmitSignal(SignalName.OnNodeDetected, node);
        EmitDebugOutput();
    }

    private void OnNodeExitedVision(Node2D node)
    {
        DetectedEntities.Remove(node);
        EmitSignal(SignalName.OnNodeLost, node);
        EmitDebugOutput();
    }

    public bool CanSee(Node2D node)
    {
        if (DetectedEntities.Contains(node)) return true;

        return false;
    }

    private void EmitDebugOutput()
    {
        if (DetectedEntities.Count() == 0) return;
        EmitSignal(SignalName.DebugOutput, "[" + string.Join(", ", DetectedEntities.Select(e => e.Name)) + "]");
    }

    public System.Collections.Generic.List<T> GetDetectedInstancesOf<T>() where T : Node
    {
        return [.. DetectedEntities.OfType<T>().ToArray()];
    }



    public T GetClosestDetectedInstanceOf<[MustBeVariant] T>(T[] options) where T : Node2D
    {
        System.Collections.Generic.List<T> optionsDetected = GetDetectedInstancesOf<T>();
        if (optionsDetected.Count == 0) return null;

        T[] optionsDetectedAndAvailable = [.. optionsDetected.Intersect(options)];
        if (optionsDetectedAndAvailable.Length == 0) return null;

        return GetClosestInstanceOf(optionsDetectedAndAvailable);
    }
    public System.Collections.Generic.List<T> GetDetectedInstancesImplementing<T>() where T : class
    {
        return DetectedEntities
            .OfType<Node>()
            .Where(node => node is T)
            .Cast<T>()
            .ToList();
    }

    public T GetClosestInstanceOf<[MustBeVariant] T>(T[] options, Vector2? source = null) where T : Node2D
    {
        if (options == null || options.Length == 0) return null;

        Vector2 agentPosition = source ?? GlobalPosition;
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
