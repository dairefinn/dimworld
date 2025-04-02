namespace Dimworld.GOAP;

using Godot;


[GlobalClass]
public partial class GoapGoal : Resource
{

    [Export] public string Name { get; set; } = "GOAP Goal";

    [Export] public int Priority { get; set; } = 0;

    [Export] public GoapState DesiredState { get; set; } = new GoapState();

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
