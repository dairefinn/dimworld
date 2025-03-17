namespace Dimworld;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GoapGoal : Resource
{

    [Export] public string Name { get; set; }

    [Export] public int Priority { get; set; }

    [Export] public Dictionary<string, Variant> DesiredState { get; set; }

    public bool IsValid()
    {
        return true;
    }

    public bool IsSatisfied(Dictionary<string, Variant> worldState, AgentBrain agentBrain)
    {
        bool worldStateSatisfied = GoapStateUtils.IsSubsetOf(DesiredState, worldState);
        bool isConditionalSatisfied = IsSatisfiedConditional(worldState, agentBrain);
        return worldStateSatisfied && isConditionalSatisfied;
    }

    public virtual bool IsSatisfiedConditional(Dictionary<string, Variant> worldState, AgentBrain agentBrain)
    {
        return true;
    }

}
