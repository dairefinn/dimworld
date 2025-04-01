namespace Dimworld;

using System.Linq;
using Dimworld.MemoryEntries;
using Godot;
using Godot.Collections;


// TODO: Might be able to make this more generic. For example, if an action calls for "has_items": [ "item-sword" ], then this action could be built dynamically to fetch a sword item from a nearby chest.
public partial class EquipSword : GoapAction
{

    [Export] public InventoryItem swordItem;

    private Chest detectedChest;
    private bool actionStarted = false;

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent)
    {
        // Sword item is required
        if (swordItem == null) {
            GD.PushError("EquipSword action requires a sword item to be set.");
            return false;
        }

        if (goapAgent is not CharacterController characterController) return false;

        // Cannot pick up a sword if the agent's inventory is full
        if (characterController.Inventory.IsFull()) return false;

        // Get any nearby chests that can be reached
        Array<Chest> detectedChests = [..
            characterController.DetectionHandler.GetDetectedInstancesOf<Chest>()
            .Where(chest => {
                if (!characterController.CanReachPoint(chest.GlobalPosition)) return false;
                if (!chest.ContainsItem(swordItem)) return false;
                return true;
            })
            .ToArray()
        ];
        if (detectedChests.Count == 0) return false;

        // Get the closest chest
        Vector2 agentPosition = characterController.GlobalPosition;
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

    public override bool Perform(IGoapAgent goapAgent, Dictionary<string, Variant> worldState, double delta)
    {
        if (detectedChest == null) return false;
        if (goapAgent is not CharacterController characterController) return false;
        
        characterController.NavigateTo(detectedChest.GlobalPosition);

        if(characterController.NavigationAgent.IsNavigationFinished())
        {
            InventorySlot agentSlot = characterController.Inventory.GetFirstEmptySlot();
            InventorySlot chestSlot = detectedChest.Inventory.GetFirstSlotWithItem(swordItem);
            agentSlot.SwapWithExisting(chestSlot);
            characterController.SetInventoryState();
            characterController.MemoryHandler.AddMemory(InventoryContents.FromNode(detectedChest));
            return true;
        }

        if(!actionStarted)
        {
            characterController.Say("I'll grab a sword from this chest.");
        }
        actionStarted = true;

        return false;
    }

    public override Dictionary<string, Variant> OnEnd(IGoapAgent goapAgent, Dictionary<string, Variant> worldState)
    {
        return worldState;
    }

}
