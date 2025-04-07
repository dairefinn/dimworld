namespace Dimworld;

using System.Linq;
using System.Threading;
using Dimworld.Developer;
using Dimworld.GOAP;
using Dimworld.Helpers;
using Godot;
using Godot.Collections;


public partial class PlanningHandler : Node2D
{


    [Export] public float lookForGoalsEveryXSeconds = 0.5f;


	private float secondsToNextGoalUpdate = 0;
	private GoapGoal CurrentGoal { get; set; }
	private GoapAction[] CurrentPlan { get; set; } = [];
	private GoapAction CurrentAction { get; set; }
	private int CurrentPlanStep { get; set; } = 0;

	private Thread planningThread;
	private bool isPlanning = false;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		// Ensure the thread is stopped when the node is removed
		if (planningThread != null && planningThread.IsAlive)
		{
			// DeveloperConsole.Print("Updating current plan");
			// UpdateCurrentPlan(agent);
			planningThread.Join();
		}
	}

	public void OnProcess(IGoapAgent agent, double delta)
	{
		if (secondsToNextGoalUpdate <= 0 && !isPlanning)
		{
			DeveloperConsole.Print("Starting planning thread");
			isPlanning = true;
			planningThread = new Thread(() => ThreadedUpdateCurrentPlan(agent));
			planningThread.Start();
			secondsToNextGoalUpdate = lookForGoalsEveryXSeconds;
		}

		FollowPlan(agent, CurrentPlan, delta);
		secondsToNextGoalUpdate -= (float)delta;
	}

	private void ThreadedUpdateCurrentPlan(IGoapAgent agent)
	{
		Array<GoapGoal> goalsInOrder = GoapPlanner.GetGoalsInOrder(agent.GoalSet);

		foreach (GoapGoal goal in goalsInOrder)
		{
			if (!goal.IsValid()) continue;
			if (goal.IsSatisfied(agent.WorldState, agent)) continue;

			GoapAction[] planForGoal = GoapPlanner.GetPlan(goal, agent);
			if (planForGoal.Length == 0) continue;

			// If nothing has changed, don't update the plan
			if (CurrentGoal != null && CurrentGoal.Name == goal.Name) break;
			if(CurrentPlan == planForGoal) break;

			DeveloperConsole.Print("New goal: " + goal.Name);
			DeveloperConsole.Print("Plan: [" + string.Join(", ", planForGoal.Select(action => action.Name)) + "]");

			// Update shared variables in a thread-safe way
			CallDeferred(MethodName.UpdatePlan, [goal, planForGoal]);
			break;
		}

		isPlanning = false;
	}

	// Thread-safe method to update the plan
	private void UpdatePlan(GoapGoal goal, GoapAction[] plan)
	{
		CurrentGoal = goal;
		CurrentPlan = plan;
		CurrentAction = null;
		CurrentPlanStep = 0;
	}

	private void FollowPlan(IGoapAgent agent, GoapAction[] plan, double delta)
	{
		if (plan == null || plan.Length == 0) return;

		GoapAction actionAtStep = plan[CurrentPlanStep];

		// If the agent is starting a new action, run the OnStart lifecycle event
		if (CurrentAction != actionAtStep)
		{
			DeveloperConsole.Print($"New action: {BBCodeHelper.Colors.Yellow(actionAtStep.Name)}");
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
