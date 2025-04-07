namespace Dimworld.Modifiers;

using System;
using Godot;


public abstract partial class AgentStatsModifier : Modifier
{

    public AgentStats TargetStats = null;

    public AgentStatsModifier(string key) : base(key)
    {
    }


    public AgentStatsModifier SetTargetStats(AgentStats targetStats)
    {
        TargetStats = targetStats;
        return this;
    }


    public override void OnAdded(ModifierHandler handler)
    {
        base.OnAdded(handler);

        if (!IsInstanceValid(handler))
        {
            GD.PrintErr($"Modifier handler is not valid: {handler}");
            throw new Exception("Failed to add modifier: handler is not valid");
        }

        Node parent = handler.GetParent();

        if (parent is not IHasAgentStats nodeWithStats)
        {
            GD.PrintErr($"Parent is not a CharacterController: {parent}");
            throw new Exception("Failed to add modifier: parent is not a CharacterController");
        }

        if (nodeWithStats.Stats == null)
        {
            GD.PrintErr($"TargetStats is null for modifier: {this}");
            throw new Exception("Failed to add modifier: TargetStats is null");
        }

        TargetStats = nodeWithStats.Stats;
    }


}
