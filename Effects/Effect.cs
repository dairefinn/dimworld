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
    /// The collision shape of the effect. This is the hitbox shape that will be used to detect collisions with other nodes.
    /// </summary>
    private CollisionShape2D _collisionShape = null;


    // CONSTRUCTORS AND BUILDERS

    public Effect(Shape2D hitboxShape, uint collisionlayer) // TODO: uint is too confusing to use here
    {
        Name = GetType().Name;
        _collisionShape = new CollisionShape2D
        {
            Name = "Hitbox",
            Shape = hitboxShape
        };

        CollisionLayer = 0;
        CollisionMask = collisionlayer;

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


    // LIFECYCLE EVENTS
    
    public override void _EnterTree()
    {
        base._EnterTree();

        // This ensures that the collision shape node is only added to the scene AFTER the effect node is
        AddChild(_collisionShape);
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
        DetectedNodes.Remove(node);
    }


    // DETECTED AREAS MANAGEMENT

    public virtual void AddDetectedArea(Area2D area)
    {
        DetectedAreas.Add(area);
    }

    public virtual void RemoveDetectedArea(Area2D area)
    {
        DetectedAreas.Remove(area);
    }

}
