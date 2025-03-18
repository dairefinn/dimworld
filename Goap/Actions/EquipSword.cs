namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class EquipSword : GoapAction
{

    private Sword detectedSword;

    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        detectedSword = agentBrain.DetectedEntities.OfType<Sword>().FirstOrDefault();
        if (detectedSword != null)
        {
            // GD.Print("Can see sword");
            return true;
        }
        else
        {
            // GD.Print("Cannot see sword");
            return false;
        }
    }


    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        if (detectedSword == null) return false;
        
        agentBrain.Agent.NavigateTo(detectedSword.GlobalPosition);

        if(agentBrain.Agent.NavigationAgent.IsNavigationFinished())
        {
            detectedSword.QueueFree();
            GoapStateUtils.SetState(worldState, "sword_equipped", true);
            return true;
        }

        return false;
    }

    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        return worldState;
    }

}
