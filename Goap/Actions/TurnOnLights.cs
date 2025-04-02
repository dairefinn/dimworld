namespace Dimworld.GOAP.Actions;

using System.Linq;
using Dimworld.MemoryEntries;
using Godot;
using Godot.Collections;


public partial class TurnOnLights : GoapAction
{

    private LightSwitch detectedLightSwitch = null;
    private bool actionStarted = false;

    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"can_see", true}
        });
    }

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        if (goapAgent is not CharacterController characterController) return false;

        // Get any lightbulbs in the correct state
        Array<LightBulb> lightBulbs = [..characterController.MemoryHandler.GetMemoriesOfType<NodeLocation>()
            .Where(memory => memory.Node is LightBulb)
            .Select(memory => (LightBulb)memory.Node)
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

        // If the agent cannot see the light switch that controls the light, check their memory
        if (lightSwitches.Count == 0)
        {
            GD.Print("Light bulb is off but agent cannot see the light switch, checking memory");
            LightSwitch[] lightSwitchesInMemory = characterController.MemoryHandler.GetMemoriesOfType<NodeLocation>()
                .Where(memory => memory.Node is LightSwitch)
                .Select(memory => (LightSwitch)memory.Node)
                .ToArray();

            lightSwitches = [..lightSwitchesInMemory
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
        }

        if (lightSwitches.Count == 0) return false; // No light switches found, cannot perform action
        GD.Print("Light switch found, checking if agent can reach it");

        detectedLightSwitch = characterController.DetectionHandler.GetClosestInstanceOf(lightSwitches);
        if (detectedLightSwitch == null) return false;

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false;

        if (!actionStarted)
        {
            characterController.Say("I need to turn on the lights");
        }
        actionStarted = true;

        characterController.NavigateTo(detectedLightSwitch.GlobalPosition);

        if(characterController.NavigationAgent.IsNavigationFinished())
        {
            GD.Print("Reached light switch, toggling it");
            detectedLightSwitch.Toggle();
            DeleteMemoryOfAssociatedLights(characterController, detectedLightSwitch);
        }

        return detectedLightSwitch.IsOn;
    }

    private void DeleteMemoryOfAssociatedLights(CharacterController characterController, LightSwitch lightSwitch)
    {
        Array<LightBulb> bulbs = lightSwitch.AssociatedLights;

        foreach (LightBulb lightBulb in bulbs)
        {
            if (lightBulb is not IMemorableNode memorableNode) continue;
            NodeLocation memoryEntry = memorableNode.GetNodeLocationMemory();
            MemoryEntry memoryEntryExisting = memoryEntry.GetMatchingEntryFrom(characterController.MemoryHandler.MemoryEntries);
            if (memoryEntryExisting == null) continue; // Only update memory for known lightbulbs
            GD.Print($"Deleting memory of lightbulb {lightBulb.Name}");
            characterController.MemoryHandler.AddMemory(memoryEntry);
        }
    }

}
