namespace Dimworld.GOAP.Actions;

using System.Linq;
using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Developer;
using Dimworld.Core.GOAP;
using Dimworld.Entities.Objects;
using Godot;
using Godot.Collections;


public partial class TurnOnLights : GoapAction
{

    private LightSwitch detectedLightSwitch = null;
    private bool actionStarted = false;
    private bool targetBulbState = false;

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
        System.Collections.Generic.List<LightBulb> lightBulbs = [..characterController.MemoryHandler.GetMemoriesOfType<NodeLocation>()
            .Where(memory => memory.Node is LightBulb)
            .Where(memory => characterController.CanReachPoint(memory.Position))
            .Select(memory => (LightBulb)memory.Node)
            .Where(lightBulb => lightBulb.IsOn == targetBulbState)
            .ToArray()
        ];

        if (lightBulbs.Count == 0) return false;

        // Get any light switches that control the lightbulbs
        System.Collections.Generic.List<LightSwitch> lightSwitches = [..characterController.MemoryHandler.GetMemoriesOfType<NodeLocation>()
            .Where(memory => memory.Node is LightSwitch)
            .Where(memory => characterController.CanReachPoint(memory.Position))
            .OrderBy(memory => memory.Position.DistanceTo(goapAgent.GlobalPositionThreadSafe))
            .Select(memory => (LightSwitch)memory.Node)
            .Where(lightSwitch => {
                foreach (LightBulb lightBulb in lightBulbs)
                {
                    if (lightSwitch.ControlsLight(lightBulb)) return true;
                }
                return false;
            })
            .ToArray()
        ];

        if (lightSwitches.Count == 0) return false; // No light switches found, cannot perform action
        DeveloperConsole.Print("Light switch found, checking if agent can reach it");

        detectedLightSwitch = lightSwitches.First();
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
            DeveloperConsole.Print("Reached light switch, toggling it");
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
            DeveloperConsole.Print($"Deleting memory of lightbulb {lightBulb.Name}");
            characterController.MemoryHandler.AddMemory(memoryEntry);
        }
    }

}
