namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class TurnOnLights : GoapAction
{

    private Light2D Light = null;


    public override Dictionary<string, Variant> OnStart(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        Light = agentBrain.GetTree().GetNodesInGroup("lights").OfType<Light2D>().FirstOrDefault();
        GoapStateUtils.SetState(worldState, "lights_on", Light.Enabled);
        return worldState;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        if (Light == null)
        {
            GD.Print("No lights found");
            return false;
        }

        agentBrain.Agent.NavigateTo(Light.GlobalPosition);

        if(agentBrain.Agent.NavigationAgent.IsNavigationFinished())
        {
            Light.Enabled = true;
            GoapStateUtils.SetState(worldState, "lights_on", Light.Enabled);
        }

        return Light.Enabled;
    }
    
    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        GoapStateUtils.SetState(worldState, "lights_on", Light.Enabled);
        return worldState;
    }

}