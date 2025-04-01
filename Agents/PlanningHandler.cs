namespace Dimworld;

using System.Linq;
using Godot;


public partial class PlanningHandler : Node2D
{


    [Export] public int lookForGoalsEveryXFrames = 60;


	private int framesToNextGoalUpdate = 0;
	private GoapGoal CurrentGoal { get; set; }
	private GoapAction[] CurrentPlan { get; set; } = [];
	private GoapAction CurrentAction { get; set; }
	private int CurrentPlanStep { get; set; } = 0;


    public void OnProcess(IGoapAgent agent, double delta)
    {
		if (framesToNextGoalUpdate == 0)
		{
			UpdateCurrentPlan(agent);
			framesToNextGoalUpdate = lookForGoalsEveryXFrames + 1;
		}
		FollowPlan(agent, CurrentPlan, delta);
		framesToNextGoalUpdate--;

    }


	private void UpdateCurrentPlan(IGoapAgent agent)
	{
		GoapGoal[] goalsInOrder = GoapPlanner.GetGoalsInOrder(agent.GoalSet.ToArray(), agent.WorldState, agent);

		foreach (GoapGoal goal in goalsInOrder)
		{
			if (!goal.IsValid()) continue;
			if (goal.IsSatisfied(agent.WorldState, agent)) continue;

			GoapAction[] planForGoal = GoapPlanner.GetPlan(goal, agent.WorldState, agent.ActionSet.ToArray(), agent);
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
