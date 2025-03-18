namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class EquipSword : GoapAction
{

    private Chest detectedChest;

    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        // Get any nearby chests that can be reached
        Array<Chest> detectedChests = [..
            agentBrain.DetectionHandler.GetDetectedInstancesOf<Chest>()
            .Where(chest => agentBrain.MovementController.CanReachPoint(chest.GlobalPosition))
            // TODO: Check if chest contains a sword once the inventory system is implemented
            .ToArray()
        ];
        if (detectedChests.Count == 0) return false;

        // Get the closest chest
        Vector2 agentPosition = agentBrain.MovementController.GlobalPosition;
        detectedChest = detectedChests[0];
        for(int i = 1; i < detectedChests.Count; i++)
        {
            Chest sword = detectedChests[i];
            if (agentPosition.DistanceTo(sword.GlobalPosition) < agentPosition.DistanceTo(detectedChest.GlobalPosition))
            {
                detectedChest = sword;
            }
        }

        // If no chest is found, return false
        if (detectedChest == null) return false;
        
        return true;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        if (detectedChest == null) return false;
        
        agentBrain.MovementController.NavigateTo(detectedChest.GlobalPosition);

        if(agentBrain.MovementController.NavigationAgent.IsNavigationFinished())
        {
            detectedChest.QueueFree();
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
