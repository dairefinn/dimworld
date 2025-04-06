namespace Dimworld.Effects;

using Godot;


/// <summary>
/// This effect will apply a push or pull effect to all nodes that enter the area. The effect will be applied every interval until the effect expires or is removed.
/// </summary>
public partial class PushPullEffect : MovementEffect
{

    public enum Direction
    {
        PUSH,
        PULL
    }


    private float _force = 0f;
    private Direction _direction = Direction.PUSH;


    public PushPullEffect(Shape2D hitboxShape, int[] collisionlayers, float force) : base(hitboxShape, collisionlayers)
    {
        _force = force;
    }


    public PushPullEffect SetDirection(Direction direction)
    {
        _direction = direction;
        return this;
    }


    public override void TriggerEffect(double delta)
    {
        foreach(Node node in DetectedNodes)
        {
            if (node is not ICanBeMoved nodeCanBeMoved) return; // Node must have ICanBeMoved

            Vector2 nodePosition = nodeCanBeMoved.GetGlobalPosition();
            Vector2 direction = GlobalPosition.DirectionTo(nodePosition);

            if (_direction == Direction.PULL)
            {
                direction = direction.Rotated(Mathf.DegToRad(180));
            }

            Vector2 knockbackVelocity = direction * _force;
            nodeCanBeMoved.ApplyVelocity(knockbackVelocity, delta);
        }
    }

}
