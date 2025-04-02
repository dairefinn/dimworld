namespace Dimworld;

using System.Linq;
using Dimworld.GOAP;
using Godot;
using Godot.Collections;

public partial class PlanningHandler : Node2D
{


    [Export] public int lookForGoalsEveryXSeconds = 2;


	private float secondsToNextGoalUpdate = 0;
	private GoapGoal CurrentGoal { get; set; }
	private GoapAction[] CurrentPlan { get; set; } = [];
	private GoapAction CurrentAction { get; set; }
	private int CurrentPlanStep { get; set; } = 0;


    public void OnProcess(IGoapAgent agent, double delta)
    {
		if (secondsToNextGoalUpdate <= 0)
		{
			GD.Print("Updating current plan");
			UpdateCurrentPlan(agent);
			secondsToNextGoalUpdate = lookForGoalsEveryXSeconds;
		}
		FollowPlan(agent, CurrentPlan, delta);
		secondsToNextGoalUpdate -= (float)delta;
    }


	private void UpdateCurrentPlan(IGoapAgent agent)
	{
		Array<GoapGoal> goalsInOrder = GoapPlanner.GetGoalsInOrder(agent.GoalSet);

		foreach (GoapGoal goal in goalsInOrder)
		{
			if (!goal.IsValid()) continue;
			if (goal.IsSatisfied(agent.WorldState, agent)) continue;

			GoapAction[] planForGoal = GoapPlanner.GetPlan(goal, agent);
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


	private void FollowPlan(IGoapAgent agent, GoapAction[] plan, double delta)
	{
		if (plan == null || plan.Length == 0) return;

		GoapAction actionAtStep = plan[CurrentPlanStep];

		// If the agent is starting a new action, run the OnStart lifecycle event
		if (CurrentAction != actionAtStep)
		{
			GD.Print("New action: " + actionAtStep.Name);
			CurrentAction?.OnEnd(agent, agent.WorldState);
			CurrentAction = actionAtStep;
			CurrentAction.OnStart(agent, agent.WorldState);
		}

		// Perform the current action step
		bool isStepComplete = CurrentAction.Perform(agent, agent.WorldState, delta);

		// If the action is complete:
		// - Run the OnEnd lifecycle event
		// - Move to the next step in the plan
		if (isStepComplete)
		{
			// Run the OnEnd lifecycle event
			CurrentAction.OnEnd(agent, agent.WorldState);

			// Move to the next step in the plan
			if (CurrentPlanStep < plan.Length - 1)
			{
				CurrentPlanStep++;
			}
			else
			{
				CurrentGoal = null;
				CurrentPlan = [];
				CurrentPlanStep = 0;
			}
		}
	}


}
