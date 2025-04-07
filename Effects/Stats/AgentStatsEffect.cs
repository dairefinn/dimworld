namespace Dimworld.Effects;

using Godot;


public abstract partial class AgentStatsEffect : Effect
{

    public AgentStatsEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not IHasAgentStats nodeHasStats) return; // Node must have AgentStats
        if (nodeHasStats.Stats == null) return; // Node must have AgentStats

        base.OnBodyEntered(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not IHasAgentStats nodeHasStats) return; // Node must have AgentStats
        if (nodeHasStats.Stats == null) return; // Node must have AgentStats

        base.OnBodyExited(body);
    }

    public override void TriggerEffectOnNode(Node node, double delta)
    {
        if (node is not IHasAgentStats nodeWithStats) return; // Node must have AgentStats
        TriggerEffectOnNode(node, nodeWithStats, delta);
    }

    public virtual void TriggerEffectOnNode(Node node, IHasAgentStats nodeWithStats, double delta)
    {
        // This method should be overridden in derived classes to apply specific effects to the node.
    }

}
