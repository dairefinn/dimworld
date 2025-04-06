namespace Dimworld.Effects;

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
    /// The start position of the effect. This is where the effect will be created.
    /// </summary>
    private Vector2 StartPosition = Vector2.Zero;

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
    private float ProcessInterval = -1f; // -1 means no interval, 0 is matched to delta

    /// <summary>
    /// Determines if the effect should be triggered instantly when a node enters the area. This is used for effects that need to be applied immediately, like a knockback effect.
    /// </summary>
    private bool TriggerInstantly = true;


    /// <summary>
    /// The collision shape of the effect. This is the hitbox shape that will be used to detect collisions with other nodes.
    /// </summary>
    private CollisionShape2D _collisionShape = null;

    private float _intervalTimerRemaining = 0f;
    private bool _triggerInstantlyRun = false;
    private bool _isReadyTimerElapsed = false;


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
        CollisionMask = ParseCollisionLayers(collisionLayers);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    /// <summary>
    /// Converts the collision layers from an array of integers to a uint. This is used to set the collision mask of the effect.
    /// </summary>
    /// <param name="layers"></param>
    /// <returns></returns>
    private uint ParseCollisionLayers(params int[] layers)
    {
        uint collisionLayers = 0;
        foreach (int layer in layers)
        {
            collisionLayers |= (uint)(1 << (layer - 1));
        }
        return collisionLayers;
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
        return this;
    }

    public Effect SetTriggerInstantly(bool triggerInstantly)
    {
        TriggerInstantly = triggerInstantly;
        return this;
    }


    // LIFECYCLE EVENTS
    
    public override void _EnterTree()
    {
        base._EnterTree();

        // This ensures that the collision shape node is only added to the scene AFTER the effect node is
        AddChild(_collisionShape);

        GlobalPosition = StartPosition; // Set the position of the effect to the start position
    }

    public override void _Ready()
    {
        base._Ready();

        _intervalTimerRemaining = ProcessInterval;

        // TODO: This is a massive hack to ensure the collision detection has had time to run before triggering the effect instantly. The timer is totally arbitrary and is not based on any real logic. There's probably a better way to do this.
        GetTree().CreateTimer(0.1f).Timeout += () => {
            _isReadyTimerElapsed = true;
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // Call the OnInterval method immediately if TriggerInstantly is true and the effect is ready to be triggered instantly
        if (_isReadyTimerElapsed && !_triggerInstantlyRun && TriggerInstantly && _collisionShape.IsNodeReady())
        {
            _triggerInstantlyRun = true;
            TriggerEffect(delta);
        }
        
        UpdateIntervalTimer(delta); // Update the timer for the interval
        UpdateDuration(delta); // Update the duration of the effect
    }


    // DURATION HANDLING

    private void UpdateDuration(double delta)
    {
        if (Duration == -1f) return; // If the duration is -1, don't do anything

        Duration -= (float)delta; // Decrease the duration by the delta time

        if (_isReadyTimerElapsed && Duration <= 0f) // If the duration is less than or equal to 0, remove the effect
        {
            QueueFree();
        }
    }


    // INTERVAL HANDLING
    
    private void UpdateIntervalTimer(double delta)
    {
        if (ProcessInterval == -1f) return; // If the interval is -1, don't do anything

        _intervalTimerRemaining -= (float)delta; // Decrease the timer by the delta time

        if (_intervalTimerRemaining <= 0f) // If the timer is less than or equal to 0, call the OnInterval method
        {
            TriggerEffect(delta);
            _intervalTimerRemaining = ProcessInterval; // Reset the timer to the interval
        }        
    }

    public virtual void TriggerEffect(double delta)
    {
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
        DetectedNodes.Add(node);
    }

    public virtual void RemoveDetectedNode(Node node)
    {
        if (Sticky) return; // If the effect is sticky, don't remove the node from the detection list
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

}
