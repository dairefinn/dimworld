namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


public partial class TurnOffLights : GoapAction
{

    private LightSwitch detectedLightSwitch = null;

    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        // Get any lightbulbs in the correct state
        Array<LightBulb> lightBulbs = [..agentBrain.DetectionHandler.GetDetectedInstancesOf<LightBulb>()
            .Where(lightBulb => lightBulb.IsOn == true)
            .ToArray()
        ];
        if (lightBulbs.Count == 0) return false;

        // Get any light switches that control the lightbulbs
        Array<LightSwitch> lightSwitches = [..agentBrain.DetectionHandler.GetDetectedInstancesOf<LightSwitch>()
            .Where(lightSwitch => {
                foreach (LightBulb lightBulb in lightBulbs)
                {
                    if (lightSwitch.ControlsLight(lightBulb)) return true;
                }
                return false;
            })
            .ToArray()
        ];
        if (lightSwitches.Count == 0) return false;

        Array<LightSwitch> reachableSwitches = [..lightSwitches
            .Where(lightSwitch => agentBrain.MovementController.CanReachPoint(lightSwitch.GlobalPosition))
            .ToArray()
        ];

        detectedLightSwitch = agentBrain.DetectionHandler.GetClosestInstanceOf(reachableSwitches);
        if (detectedLightSwitch == null) return false;

        return true;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        agentBrain.MovementController.NavigateTo(detectedLightSwitch.GlobalPosition);

        if(agentBrain.MovementController.NavigationAgent.IsNavigationFinished())
        {
            detectedLightSwitch.Toggle();
        }

        return detectedLightSwitch.IsOn;
    }

}
