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

    public bool IsSatisfied(Dictionary<string, Variant> worldState, IGoapAgent goapAgent)
    {
        bool worldStateSatisfied = GoapStateUtils.IsSubsetOf(DesiredState, worldState);
        if (!worldStateSatisfied) return false;

        bool isConditionalSatisfied = IsSatisfiedConditional(worldState, goapAgent);
        if (!isConditionalSatisfied) return false;

        return true;
    }

    public virtual bool IsSatisfiedConditional(Dictionary<string, Variant> worldState, IGoapAgent goapAgent)
    {
        return true;
    }

}
