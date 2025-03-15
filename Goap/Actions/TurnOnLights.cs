namespace Dimworld;

using System.Linq;
using Godot;


public partial class TurnOnLights : GoapAction
{

    public override bool Perform(AgentController agent, double delta)
    {
        Light2D Light = agent.GetTree().GetNodesInGroup("lights").OfType<Light2D>().FirstOrDefault();

        if (Light == null)
        {
            GD.Print("No lights found");
            return false;
        }

        agent.NavigateTo(Light.GlobalPosition);

        if(agent.NavigationAgent.IsNavigationFinished())
        {
            Light.Enabled = true;
            return true;
        }
        else
        {
            return false;
        }
    }

}