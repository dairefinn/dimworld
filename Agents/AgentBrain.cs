namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class AgentBrain : Node
{

    [Export] public AgentController Agent { get; set; }

    // Goal Oriented Action Planning (GOAP) properties
    [Export] public Dictionary<string, Variant> WorldState { get; set; }
    [Export] public Array<GoapAction> ActionSet { get; set; }
    [Export] public Array<GoapGoal> GoalSet { get; set; }

    // Sensory system properties
    [Export] public Area2D AreaVision { get; set; }
    [Export] public Area2D AreaInteraction { get; set; }


    private GoapGoal CurrentGoal { get; set; }
    private GoapAction[] CurrentPlan { get; set; } = [];
    private GoapAction CurrentAction { get; set; }
    private int CurrentPlanStep { get; set; } = 0;
    private float InteractionRadius { get; set; } = 1;
    private float VisionRadius { get; set; } = 1;


    public override void _Ready()
    {
        base._Ready();
        GetSensoryRangesFromShapes();
    }


    public override void _Process(double delta)
    {
        GoapGoal bestGoal = GoapPlanner.GetBestGoal(GoalSet.ToArray(), WorldState, this);
        if (bestGoal == null)
        {
            return;
        }

        if (CurrentGoal == null || bestGoal.Name != CurrentGoal.Name)
        {
            GD.Print("New goal: " + bestGoal.Name);
            CurrentGoal = bestGoal;
            CurrentPlan = GoapPlanner.GetPlan(bestGoal, WorldState, ActionSet.ToArray(), this);
            GD.Print("Plan: [" + string.Join(", ", CurrentPlan.Select(action => action.Name)) + "]");
            CurrentPlanStep = 0;
        }
        else
        {
            FollowPlan(CurrentPlan, delta);
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
        // - Add the action's effects to the world state
        // - Run the OnEnd lifecycle event
        // - Move to the next step in the plan
        if (isStepComplete)
        {
            // Update the world state with the action's effects
            // GoapStateUtils.Add(WorldState, CurrentAction.Effects);

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


    // SENSORY AND INTERACTION

    private void GetSensoryRangesFromShapes()
    {
        if (AreaVision != null)
        {
            if (AreaVision.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                VisionRadius = circleShape.Radius;
            }
        }

        if (AreaInteraction != null)
        {
            if (AreaInteraction.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                InteractionRadius = circleShape.Radius;
            }
        }
    }
}
