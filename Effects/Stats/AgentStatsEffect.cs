namespace Dimworld.Effects;

using Godot;


public abstract partial class AgentStatsEffect : Effect
{

    // public AgentStats TargetStats = null;

    public AgentStatsEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not IHasAgentStats characterController) return; // Node must have AgentStats
        if (characterController.Stats == null) return; // Node must have AgentStats

        AddDetectedNode(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not IHasAgentStats characterController) return; // Node must have AgentStats
        if (characterController.Stats == null) return; // Node must have AgentStats

        RemoveDetectedNode(body);
    }

}
