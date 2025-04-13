namespace Dimworld.GOAP.Actions;

using System.Linq;
using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Developer;
using Dimworld.Core.GOAP;
using Dimworld.Entities.Objects;
using Godot;
using Godot.Collections;


public partial class TurnOffLights : GoapAction
{

    private LightSwitch detectedLightSwitch = null;
    private bool actionStarted = false;
    private bool targetBulbState = true;

    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"can_see", false}
        });
    }

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        if (goapAgent is not IHasMemory hasMemory) return false;
        if (goapAgent is not IHasNavigation hasNavigation) return false;

        // Get any lightbulbs in the correct state
        System.Collections.Generic.List<LightBulb> lightBulbs = [..hasMemory.MemoryHandler.GetMemoriesOfType<NodeLocation>()
            .Where(memory => memory.Node is LightBulb)
            .Where(memory => hasNavigation.CanReachPoint(memory.Position))
            .Select(memory => (LightBulb)memory.Node)
            .Where(lightBulb => lightBulb.IsOn == targetBulbState)
            .ToArray()
        ];

        if (lightBulbs.Count == 0) return false;

        // Get any light switches that control the lightbulbs
        System.Collections.Generic.List<LightSwitch> lightSwitches = [..hasMemory.MemoryHandler.GetMemoriesOfType<NodeLocation>()
            .Where(memory => memory.Node is LightSwitch)
            .Where(memory => hasNavigation.CanReachPoint(memory.Position))
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
        if (goapAgent is not IHasNavigation hasNavigation) return false;

        if (!actionStarted && goapAgent is ICanSpeak canSpeak)
        {
            canSpeak.Say("I need to turn on the lights");
        }
        actionStarted = true;

        hasNavigation.NavigateTo(detectedLightSwitch.GlobalPosition);

        if(hasNavigation.IsTargetReached())
        {
            detectedLightSwitch.Toggle();
            DeleteMemoryOfAssociatedLights(goapAgent, detectedLightSwitch);
        }

        return detectedLightSwitch.IsOn;
    }

    private void DeleteMemoryOfAssociatedLights(IGoapAgent goapAgent, LightSwitch lightSwitch)
    {
        if (goapAgent is not IHasMemory hasMemory) return;

        Array<LightBulb> bulbs = lightSwitch.AssociatedLights;

        foreach (LightBulb lightBulb in bulbs)
        {
            if (lightBulb is not IMemorableNode memorableNode) continue;
            NodeLocation memoryEntry = memorableNode.GetNodeLocationMemory();
            MemoryEntry memoryEntryExisting = memoryEntry.GetMatchingEntryFrom(hasMemory.MemoryHandler.MemoryEntries);
            if (memoryEntryExisting == null) continue; // Only update memory for known lightbulbs
            hasMemory.MemoryHandler.AddMemory(memoryEntry);
        }
    }

}
