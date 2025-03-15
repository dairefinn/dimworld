namespace Dimworld;

using System;
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


    private GoapGoal CurrentGoal { get; set; }
    private GoapAction[] CurrentPlan { get; set; }
    private int CurrentPlanStep { get; set; } = 0;


    // TODO: Figure out when to call this method
    public override void _Process(double delta)
    {
        GoapGoal bestGoal = GoapPlanner.GetBestGoal(GoalSet.ToArray(), WorldState);

        if (CurrentGoal == null || bestGoal.Name != CurrentGoal.Name)
        {
            CurrentGoal = bestGoal;
            CurrentPlan = GoapPlanner.GetPlan(bestGoal, WorldState, ActionSet.ToArray());
            CurrentPlanStep = 0;
        }
        else
        {
            FollowPlan(CurrentPlan, delta);
        }
    }

    private void FollowPlan(GoapAction[] plan, double delta)
    {
        if (plan.Length == 0)
        {
            GD.Print("Plan: []");
            return;
        }

        GD.Print("Plan: [" + string.Join(", ", plan.Select(action => action.Name)) + "]");

        bool isStepComplete = plan[CurrentPlanStep].Perform(Agent, delta);
        if (isStepComplete)
        {
            GoapStateUtils.Add(WorldState, plan[CurrentPlanStep].Effects);
            if (CurrentPlanStep < plan.Length - 1)
            {
                CurrentPlanStep++;
            }
            else
            {
                CurrentPlan = [];
                CurrentPlanStep = 0;
            }
        }
    }

}
