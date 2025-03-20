namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class AgentBrain : Node
{

    [Export] public AgentMovementController MovementController { get; set; }
    [Export] public AgentDetectionHandler DetectionHandler { get; set; }

    // Goal Oriented Action Planning (GOAP) properties
    [Export] public Dictionary<string, Variant> WorldState { get; set; }
    [Export] public Array<GoapAction> ActionSet { get; set; }
    [Export] public Array<GoapGoal> GoalSet { get; set; }
    [Export] public bool IsPlanningEnabled { get; set; } = true;

    [Export] public Inventory Inventory { get; set; }

    private GoapGoal CurrentGoal { get; set; }
    private GoapAction[] CurrentPlan { get; set; } = [];
    private GoapAction CurrentAction { get; set; }
    private int CurrentPlanStep { get; set; } = 0;


    public int lookForGoalsEveryXFrames = 60;
    private int framesToNextGoalUpdate = 0;


    public override void _Process(double delta)
    {
        if (framesToNextGoalUpdate == 0)
        {
            UpdateCurrentPlan();
            framesToNextGoalUpdate = lookForGoalsEveryXFrames + 1;
        }
        FollowPlan(CurrentPlan, delta);
        framesToNextGoalUpdate--;

        Inventory.OnUpdated += OnInventoryUpdated;

        // Print inventory slots w/ item counts
        // if (Inventory != null)
        // {
        //     string inventoryString = "Inventory: ";
        //     System.Collections.Generic.List<string> contentsStrings = [];
        //     foreach (InventorySlot slot in Inventory.Slots)
        //     {
        //         if (slot.IsEmpty) continue;
        //         contentsStrings.Add(slot.Item.ItemName + " (" + slot.Quantity + ")");
        //     }
        //     inventoryString += string.Join(", ", contentsStrings);
        //     GD.Print(inventoryString);
        // }
    }

    private void UpdateCurrentPlan()
    {
        if (!IsPlanningEnabled) return;
        if (MovementController == null) return;

        GoapGoal[] goalsInOrder = GoapPlanner.GetGoalsInOrder(GoalSet.ToArray(), WorldState, this);

        foreach (GoapGoal goal in goalsInOrder)
        {
            GD.Print("Checking goal: " + goal.Name);
            if (!goal.IsValid()) continue;
            if (goal.IsSatisfied(WorldState, this)) continue;

            GoapAction[] planForGoal = GoapPlanner.GetPlan(goal, WorldState, ActionSet.ToArray(), this);
            if (planForGoal.Length == 0) continue;

            // If nothing has changed, don't update the plan
            if (CurrentGoal == goal) return;
            if (CurrentPlan == planForGoal) return;

            GD.Print("New goal: " + goal.Name);
            GD.Print("Plan: [" + string.Join(", ", planForGoal.Select(action => action.Name)) + "]");
            CurrentGoal = goal;
            CurrentPlan = planForGoal;
            CurrentAction = null;
            CurrentPlanStep = 0;
            return;
        }

    }


    private void FollowPlan(GoapAction[] plan, double delta)
    {
        if (plan == null || plan.Length == 0) return;

        GoapAction actionAtStep = plan[CurrentPlanStep];

        // If the agent is starting a new action, run the OnStart lifecycle event
        if (CurrentAction != actionAtStep)
        {
            GD.Print("New action: " + actionAtStep.Name);
            CurrentAction?.OnEnd(this, WorldState);
            CurrentAction = actionAtStep;
            CurrentAction.OnStart(this, WorldState);
        }

        // Perform the current action step
        bool isStepComplete = CurrentAction.Perform(this, WorldState, delta);

        // If the action is complete:
        // - Run the OnEnd lifecycle event
        // - Move to the next step in the plan
        if (isStepComplete)
        {
            // Run the OnEnd lifecycle event
            CurrentAction.OnEnd(this, WorldState);

            // Move to the next step in the plan
            if (CurrentPlanStep < plan.Length - 1)
            {
                CurrentPlanStep++;
            }
            else
            {
                CurrentPlan = []; // TODO: Is this necessary? Seems like it would just freeze the agent or be overwritten by the next plan. Maybe we should set CurrentGoal to null instead?
                CurrentPlanStep = 0;
            }
        }
    }

    private void OnInventoryUpdated()
    {
        Array<string> itemsInInventory = [];

        foreach (InventorySlot slot in Inventory.Slots)
        {
            if (slot.IsEmpty) continue;
            itemsInInventory.Add(slot.Item.id);
        }

        GD.Print("Items in inventory: " + string.Join(", ", itemsInInventory));
        WorldState["has_items"] = itemsInInventory;
    }

}
