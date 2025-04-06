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

}
