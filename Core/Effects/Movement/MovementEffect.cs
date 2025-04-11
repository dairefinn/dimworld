namespace Dimworld.Effects;

using Godot;


public abstract partial class MovementEffect : Effect
{

    public MovementEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
        SetProcessOn(ProcessingType.Physics);
        SetTriggerOn(Effect.TriggerType.Interval);
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not ICanBeMoved nodeCanBeMoved) return; // Node must have ICanBeMoved

        base.OnBodyEntered(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not ICanBeMoved nodeCanBeMoved) return; // Node must have ICanBeMoved

        base.OnBodyExited(body);
    }

    public override void TriggerEffectOnNode(Node node, double delta)
    {
        if (node is not ICanBeMoved nodeCanBeMoved) return; // Node must have AgentStats
        TriggerEffectOnNode(node, nodeCanBeMoved, delta);
    }

    public virtual void TriggerEffectOnNode(Node node, ICanBeMoved nodeCanBeMoved, double delta)
    {
        // This method should be overridden in derived classes to apply specific effects to the node.
    }
}
