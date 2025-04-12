namespace Dimworld.Effects;

using Dimworld.Core.Effects;
using Godot;


/// <summary>
/// This effect will apply a push or pull effect to all nodes that enter the area. The effect will be applied every interval until the effect expires or is removed.
/// </summary>
public partial class PushPullEffect : MovementEffect
{

    public enum Direction
    {
        PUSH,
        PULL,
        PUSH_PERPENDICULAR,
        PULL_PERPENDICULAR,
        PUSH_STRAIGHT,
        PULL_STRAIGHT
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


    public override void TriggerEffectOnNode(Node node, ICanBeMoved nodeCanBeMoved, double delta)
    {
        Vector2 nodePosition = nodeCanBeMoved.GetGlobalPosition();
        Vector2 direction = GetDirection(nodePosition);

        Vector2 knockbackVelocity = direction * _force;
        nodeCanBeMoved.ApplyVelocity(knockbackVelocity, delta);
    }

    private Vector2 GetDirection(Vector2 targetPosition)
    {
        if (_direction == Direction.PUSH_STRAIGHT)
        {
            return new Vector2(1, 0).Rotated(GlobalRotation);
        }

        if (_direction == Direction.PULL_STRAIGHT)
        {
            return new Vector2(-1, 0).Rotated(GlobalRotation);
        }

        if (_direction == Direction.PUSH_PERPENDICULAR)
        {
            return new Vector2(0, 1).Rotated(GlobalRotation);
        }

        if (_direction == Direction.PULL_PERPENDICULAR)
        {
            return new Vector2(0, -1).Rotated(GlobalRotation);
        }

        Vector2 directionToTarget = GlobalPosition.DirectionTo(targetPosition);

        if (_direction == Direction.PULL)
        {
            return directionToTarget.Rotated(Mathf.DegToRad(180));
        }

        return directionToTarget;
    }

}
