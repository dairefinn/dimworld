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
    [Export] public Array<Node> DetectedEntities { get; set; } = [];
    [Export] public Array<Marker2D> PatrolPath { get; set; } = [];


    private GoapGoal CurrentGoal { get; set; }
    private GoapAction[] CurrentPlan { get; set; } = [];
    private GoapAction CurrentAction { get; set; }
    private int CurrentPlanStep { get; set; } = 0;
    private float InteractionRadius { get; set; } = 1;
    private float VisionRadius { get; set; } = 1;


    public override void _Ready()
    {
        base._Ready();
        InitSensorySystem();
    }

    public override void _Process(double delta)
    {
        UpdateCurrentPlan();
        FollowPlan(CurrentPlan, delta);
    }

    private void UpdateCurrentPlan()
    {
        GoapGoal[] goalsInOrder = GoapPlanner.GetGoalsInOrder(GoalSet.ToArray(), WorldState, this);

        foreach (GoapGoal goal in goalsInOrder)
        {
            if (!goal.IsValid()) continue;
            if (goal.IsSatisfied(WorldState, this)) continue;

            GoapAction[] planForGoal = GoapPlanner.GetPlan(goal, WorldState, ActionSet.ToArray(), this);
            if (planForGoal.Length == 0) continue;

            // If nothing has changed, don't update the plan
            if (CurrentGoal == goal) return;
            if (CurrentPlan == planForGoal) return;

            GD.Print("New goal: " + goal.Name);
            GD.Print("Plan: [" + string.Join(", ", CurrentPlan.Select(action => action.Name)) + "]");
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


    // SENSORY AND INTERACTION

    private void InitSensorySystem()
    {
        if (AreaVision != null)
        {
            if (AreaVision.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                VisionRadius = circleShape.Radius;
            }

            AreaVision.BodyEntered += OnNodeEnteredVision;
            AreaVision.BodyExited += OnNodeExitedVision;
        }

        // TODO: This will be used to determine if an entity is close enough to "interact" with
        // It might just be better to check if you can see it and then if it's close enough distance wise
        // instead of requiring the CollisionShape.
        if (AreaInteraction != null)
        {
            if (AreaInteraction.GetChild<CollisionShape2D>(0).Shape is CircleShape2D circleShape)
            {
                InteractionRadius = circleShape.Radius;
            }
        }
    }

    private void OnNodeEnteredVision(Node2D node)
    {
        DetectedEntities.Add(node);
        UpdateDebugText();
    }

    private void OnNodeExitedVision(Node2D node)
    {
        DetectedEntities.Remove(node);
        UpdateDebugText();
    }

    // TODO: For debugging, remove after
    private void UpdateDebugText()
    {
        if (DetectedEntities.Count() > 0)
        {
            Agent.TextLabel.Text = "[" + string.Join(", ", DetectedEntities.Select(e => e.Name)) + "]";
        }
        else
        {
            Agent.TextLabel.Text = "[]";
        }
    }
}
