namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class EquipSword : GoapAction
{
    
    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        // TODO: Navigate to the sword and pick it up
        return true;
    }

    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        GoapStateUtils.SetState(worldState, "sword_equipped", true);
        return worldState;
    }

}
