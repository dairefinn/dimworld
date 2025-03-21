namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


public partial class TurnOnLights : GoapAction
{

    private LightSwitch detectedLightSwitch = null;

    public override bool CheckProceduralPrecondition(CharacterController characterController)
    {
        // Get any lightbulbs in the correct state
        Array<LightBulb> lightBulbs = [..characterController.DetectionHandler.GetDetectedInstancesOf<LightBulb>()
            .Where(lightBulb => !lightBulb.IsOn)
            .ToArray()
        ];
        if (lightBulbs.Count == 0) return false;

        // Get any light switches that control the lightbulbs
        Array<LightSwitch> lightSwitches = [..characterController.DetectionHandler.GetDetectedInstancesOf<LightSwitch>()
            .Where(lightSwitch => {
                foreach (LightBulb lightBulb in lightBulbs)
                {
                    if (lightSwitch.ControlsLight(lightBulb)) return true;
                }
                return false;
            })
            .Where(lightSwitch => characterController.CanReachPoint(lightSwitch.GlobalPosition))
            .ToArray()
        ];
        if (lightSwitches.Count == 0) return false;

        detectedLightSwitch = characterController.DetectionHandler.GetClosestInstanceOf(lightSwitches);
        if (detectedLightSwitch == null) return false;

        return true;
    }

    public override bool Perform(CharacterController characterController, Dictionary<string, Variant> worldState, double delta)
    {
        characterController.NavigateTo(detectedLightSwitch.GlobalPosition);

        if(characterController.NavigationAgent.IsNavigationFinished())
        {
            detectedLightSwitch.Toggle();
        }

        return detectedLightSwitch.IsOn;
    }

}
