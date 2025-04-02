namespace Dimworld.GOAP;

using Godot;


[GlobalClass]
public partial class GoapGoal : Resource
{

    [Export] public string Name { get; set; }

    [Export] public int Priority { get; set; }

    [Export] public GoapState DesiredState { get; set; }

    public bool IsValid()
    {
        return true;
    }

    public bool IsSatisfied(GoapState worldState, IGoapAgent goapAgent)
    {
        if (DesiredState == null) return false;

        bool worldStateSatisfied = DesiredState.IsSubsetOf(worldState);
        if (!worldStateSatisfied) return false;

        return true;
    }

}
