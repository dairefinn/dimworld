namespace Dimworld.Effects;

using Dimworld.Developer;
using Dimworld.Helpers;
using Godot;
using Godot.Collections;


public partial class Effect : Area2D
{

    public Array<Node> DetectedNodes = [];
    public Array<Area2D> DetectedAreas = [];


    /// <summary>
    /// The velocity of the effect. This determines what direction it will move in and how fast.
    /// </summary>
    private Vector2 Velocity = Vector2.Zero;

    /// <summary>
    /// The amount the scale will change over time. This is used for effects that need to grow or shrink over time.
    /// </summary>
    private Vector2 ScaleChange = Vector2.One;

    /// <summary>
    /// The start position of the effect. This is where the effect will be created.
    /// </summary>
    private Vector2? StartPosition = null;

    /// <summary>
    /// How long the effect will last in seconds. If the duration is -1, it means that the effect will not expire.
    /// </summary>
    private float Duration = -1f; // -1 means infinite

    /// <summary>
    /// A list of nodes that this effect will ignore. e.g. If the player swings a sword, the resulting damage effect should not hit the player.
    /// </summary>
    private Array<Node> NodeBlacklist = [];

    /// <summary>
    /// Determines if detected nodes should be removed from the detection list when they exit the area. Useful for effects that should stick to the node, like a fire effect.
    /// </summary>
    private bool Sticky = false;

    /// <summary>
    /// Determines how often the effect will be applied to detected nodes. This is used for effects that need to be applied over time, like damage over time or healing over time.
    /// </summary>
    private float ProcessInterval = 0; // 0 means processing rate is matched to frame rate.

    /// <summary>
    /// Determines if processing should be done in _Process or _PhysicsProcess. Effects that move objects should use Physics processing while effects that don't need to move should use Frame processing.
    /// </summary>
    private ProcessingType ProcessOn = ProcessingType.Frame;

    /// <summary>
    /// Determines when the effect will trigger.
    /// - TriggerType.Enter will cause effects to trigger when a node enters the area.
    /// - TriggerType.Exit will cause effects to trigger when a node exits the area.
    /// - TriggerType.Interval will cause effects to trigger at a set interval.
    /// </summary>
    private TriggerType TriggerOn = TriggerType.Enter;


    /// <summary>
    /// The collision shape of the effect. This is the hitbox shape that will be used to detect collisions with other nodes.
    /// </summary>
    private CollisionShape2D _collisionShape = null;
    private float _intervalTimerRemaining = 0f;
    private bool _isReadyTimerElapsed = false;
    private bool _firstIntervalRun = false;


    // CONSTRUCTORS AND BUILDERS

    public Effect(Shape2D hitboxShape, int[] collisionLayers)
    {
        Name = GetType().Name;
        _collisionShape = new CollisionShape2D
        {
            Name = "Hitbox",
            Shape = hitboxShape
        };

        CollisionLayer = 0;
        CollisionMask = UIntHelper.ParseCollisionLayers(collisionLayers);

        ZIndex = 1;

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    public Effect SetVelocity(Vector2 velocity)
    {
        Velocity = velocity;
        return this;
    }

    public Effect SetStartPosition(Vector2 startPosition)
    {
        StartPosition = startPosition;
        return this;
    }

    /// <summary>
    /// Sets the duration of the effect. -1 is infinite. 0 is instant.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public Effect SetDuration(float duration)
    {
        Duration = duration;
        return this;
    }

    public Effect SetNodeBlacklist(Array<Node> nodeBlacklist)
    {
        NodeBlacklist = nodeBlacklist;
        return this;
    }

    public Effect AddToNodeBlacklist(Node node)
    {
        NodeBlacklist.Add(node);
        return this;
    }

    public Effect RemoveFromNodeBlacklist(Node node)
    {
        NodeBlacklist.Remove(node);
        return this;
    }

    public Effect SetSticky(bool sticky)
    {
        Sticky = sticky;
        return this;
    }

    public Effect SetInterval(float interval)
    {
        ProcessInterval = interval;
        SetTriggerOn(TriggerType.Interval);
        return this;
    }

    public Effect SetTriggerOn(TriggerType triggerOn)
    {
        TriggerOn = triggerOn;
        return this;
    }

    public Effect SetProcessOn(ProcessingType processOn)
    {
        ProcessOn = processOn;
        return this;
    }

    public Effect SetScaleChange(Vector2 scaleChange)
    {
        ScaleChange = scaleChange;
        return this;
    }


    // LIFECYCLE EVENTS
    
    public override void _EnterTree()
    {
        base._EnterTree();

        // This ensures that the collision shape node is only added to the scene AFTER the effect node is
        AddChild(_collisionShape);

        if (StartPosition != null)
        {
            GlobalPosition = StartPosition.Value;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        // TODO: This is a massive hack to ensure the collision detection and trigger is run at least once. It makes sure the effect exists for at least 0.1 seconds before it is destroyed.
        GetTree().CreateTimer(0.1f).Timeout += () => {
            _isReadyTimerElapsed = true;
        };

        DeveloperConsole.Print($"Effect {Name} created with duration {Duration} seconds and processing type {ProcessOn}. Effects will be triggered on {TriggerOn}.");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (ProcessOn == ProcessingType.Frame)
        {
            ProcessNodes(delta);
        }

        UpdateDuration(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        UpdateScale(delta);

        if (ProcessOn == ProcessingType.Physics)
        {
            ProcessNodes(delta);
        }
        
        UpdateVelocity(delta);
    }

    private void ProcessNodes(double delta)
    {
        if (TriggerOn != TriggerType.Interval) return; // Only process nodes if the trigger type is interval

        if (_isReadyTimerElapsed && !_firstIntervalRun)
        {
            TriggerEffectOnNodes(delta); // Call the effect on all detected nodes immediately
            _firstIntervalRun = true; // Set the flag to true so it doesn't run again
        }

        UpdateIntervalTimer(delta);
    }


    // SCALE HANDLING

    private void UpdateScale(double delta)
    {
        if (ScaleChange == Vector2.One) return; // If the scale change is 1, don't do anything

        Scale += ScaleChange * (float)delta;

        if (Scale.X <= 0f || Scale.Y <= 0f) // If the scale is less than or equal to 0, remove the effect
        {
            CallDeferred(MethodName.QueueFree);
        }
    }


    // VELOCITY HANDLING

    private void UpdateVelocity(double delta)
    {
        if (Velocity == Vector2.Zero) return;

        GlobalPosition += Velocity * (float)delta;
    }


    // DURATION HANDLING

    private void UpdateDuration(double delta)
    {
        if (Duration == -1f) return; // If the duration is -1, don't do anything

        Duration -= (float)delta;

        if (_isReadyTimerElapsed && Duration <= 0f) // If the duration is less than or equal to 0, remove the effect
        {
            CallDeferred(MethodName.QueueFree);
        }
    }


    // INTERVAL HANDLING
    
    private void UpdateIntervalTimer(double delta)
    {
        _intervalTimerRemaining -= (float)delta;

        if (_intervalTimerRemaining <= 0f) // If the timer is less than or equal to 0, call the OnInterval method
        {
            TriggerEffectOnNodes(delta);
            _intervalTimerRemaining = ProcessInterval; // Reset the timer
        }
    }

    public virtual void TriggerEffectOnNodes(double delta)
    {
        foreach(Node node in DetectedNodes)
        {
            TriggerEffectOnNode(node, delta);
        }
    }

    public virtual void TriggerEffectOnNode(Node node, double delta)
    {
        // This method should be overridden in derived classes to apply specific effects to the node.
    }


    // SIGNAL HANDLERS

    public virtual void OnBodyEntered(Node body)
    {
        if (NodeBlacklist.Contains(body)) return; // Ignore the body if it's in the blacklist

        AddDetectedNode(body);
    }

    public virtual void OnBodyExited(Node body)
    {
        if (NodeBlacklist.Contains(body)) return; // Ignore the body if it's in the blacklist

        RemoveDetectedNode(body);
    }

    public virtual void OnAreaEntered(Area2D area)
    {
        if (NodeBlacklist.Contains(area)) return; // Ignore the area if it's in the blacklist

        AddDetectedArea(area);
    }

    public virtual void OnAreaExited(Area2D area)
    {
        if (NodeBlacklist.Contains(area)) return; // Ignore the area if it's in the blacklist

        RemoveDetectedArea(area);
    }


    // DETECTED NODES MANAGEMENT

    public virtual void AddDetectedNode(Node node)
    {
        if (TriggerOn == TriggerType.Enter)
        {
            TriggerEffectOnNode(node, 0);
        }

        DetectedNodes.Add(node);
    }

    public virtual void RemoveDetectedNode(Node node)
    {
        if (Sticky) return; // If the effect is sticky, don't remove the node from the detection list

        if (TriggerOn == TriggerType.Exit)
        {
            TriggerEffectOnNode(node, 0);
        }

        DetectedNodes.Remove(node);
    }


    // DETECTED AREAS MANAGEMENT

    public virtual void AddDetectedArea(Area2D area)
    {
        DetectedAreas.Add(area);
    }

    public virtual void RemoveDetectedArea(Area2D area)
    {
        if (Sticky) return; // If the effect is sticky, don't remove the node from the detection list
        DetectedAreas.Remove(area);
    }

    // INTERNAL TYPES

    public enum ProcessingType
    {
        Frame,
        Physics
    }

    public enum TriggerType
    {
        Enter,
        Exit,
        Interval
    }

}
