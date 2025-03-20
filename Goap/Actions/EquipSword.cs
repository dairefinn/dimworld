namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class EquipSword : GoapAction
{

    [Export] public InventoryItem swordItem;

    private Chest detectedChest;

    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        // Sword item is required
        if (swordItem == null) {
            GD.PushError("EquipSword action requires a sword item to be set.");
            return false;
        }

        // Cannot pick up a sword if the agent's inventory is full
        if (agentBrain.Inventory.IsFull()) return false;

        // Get any nearby chests that can be reached
        Array<Chest> detectedChests = [..
            agentBrain.DetectionHandler.GetDetectedInstancesOf<Chest>()
            .Where(chest => {
                if (!agentBrain.MovementController.CanReachPoint(chest.GlobalPosition)) return false;
                if (!chest.ContainsItem(swordItem)) return false;
                return true;
            })
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
            InventorySlot agentSlot = agentBrain.Inventory.GetFirstEmptySlot();
            InventorySlot chestSlot = detectedChest.Inventory.GetFirstSlotWithItem(swordItem);
            agentSlot.AddFromExisting(chestSlot);
            return true;
        }

        return false;
    }

    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        return worldState;
    }

}
