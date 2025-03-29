namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class CharacterController : CharacterBody2D, IDamageable, ICanBeMoved
{

	[ExportGroup("Properties")]
	[Export] public float Speed { get; set; } = 50f;
	[Export] public float Acceleration { get; set; } = 25f;
	[Export] public Inventory Inventory { get; set; }
	[Export] public EquipmentHandler EquipmentHandler { get; set; }
	[Export] public AgentStats Stats {
		get => _stats;
		set => SetStats(value);
	}
	private AgentStats _stats;

	[ExportGroup("GOAP properties")]
	[Export] public bool IsPlanningEnabled { get; set; } = true;
	[Export] public Dictionary<string, Variant> WorldState { get; set; }
	[Export] public Array<GoapAction> ActionSet { get; set; }
	[Export] public Array<GoapGoal> GoalSet { get; set; }

	[ExportGroup("References")]
	[Export] public NavigationAgent2D NavigationAgent { get; set; }
	[Export] public DetectionHandler DetectionHandler { get; set; }
	[Export] public AgentStatsUI StatsUI {
		get => _statsUI;
		set => SetStatsUI(value);
	}
	private AgentStatsUI _statsUI;

	// Goal Oriented Action Planning (GOAP) properties


	private GoapGoal CurrentGoal { get; set; }
	private GoapAction[] CurrentPlan { get; set; } = [];
	private GoapAction CurrentAction { get; set; }
	private int CurrentPlanStep { get; set; } = 0;


	public int lookForGoalsEveryXFrames = 60;
	private int framesToNextGoalUpdate = 0;


	// LIFECYCLE EVENTS

	public override void _Ready()
	{
		base._Ready();

		if (Stats != null)
		{
			Stats = new AgentStats(Stats);
		}


		if (NavigationAgent == null)
		{
			NavigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		}
		NavigationAgent.VelocityComputed += OnSafeVelocityComputed;

		if (StatsUI == null)
		{
			StatsUI = GetNode<AgentStatsUI>("AgentStatsUI");
			StatsUI.Stats = Stats;
			// StatsUI.Hide();
		}

		SetInventoryState();


		GetTree().CreateTimer(3f).Timeout += () => {
			Stats.Health = Stats.MaxHealth / 3;
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		ProcessNavigation(delta);
	}

	public override void _Process(double delta)
	{
		if (framesToNextGoalUpdate == 0)
		{
			UpdateCurrentPlan();
			framesToNextGoalUpdate = lookForGoalsEveryXFrames + 1;
		}
		FollowPlan(CurrentPlan, delta);
		framesToNextGoalUpdate--;

		Inventory.OnUpdated += () => SetInventoryState();

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


	// SETTERS
	
	public void SetStatsUI(AgentStatsUI statsUI)
	{
		_statsUI = statsUI;
		LinkStatsToUI();
	}

	public void SetStats(AgentStats stats)
	{
		_stats = stats;
		LinkStatsToUI();
	}

	private void LinkStatsToUI()
	{
		if (StatsUI == null) return;
		StatsUI.Stats = Stats;
	}


	// GOAP METHODS

	private void UpdateCurrentPlan()
	{
		if (!IsPlanningEnabled) return;

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
			SetInventoryState();
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
				CurrentGoal = null;
				CurrentPlan = [];
				CurrentPlanStep = 0;
			}
		}
	}


	// AGENT MEMORY HANDLING

	// TODO: Might want to move this to a separate class, e.g. AgentMemoryHandler
	public void SetInventoryState()
	{
		if (WorldState == null)
		{
			WorldState = [];
		}

		Array<string> itemsInInventory = [];

		foreach (InventorySlot slot in Inventory.Slots)
		{
			if (slot.IsEmpty) continue;
			itemsInInventory.Add(slot.Item.Id);
			GD.Print("Item in inventory: " + slot.Item.Id);
		}

		WorldState.Remove("has_items");
		WorldState["has_items"] = itemsInInventory;
	}


	// MOVEMENT CONTROLLER

	
	// NAVIGATION

	public void NavigateTo(Vector2 target)
	{
		if (NavigationAgent.TargetPosition == target) return;
		NavigationAgent.TargetPosition = target;
	}

	public void StopNavigating()
	{
		NavigationAgent.TargetPosition = GlobalPosition;
	}

	public bool CanReachPoint(Vector2 targetPoint)
	{
		NavigationAgent2D tempNavigationAgent = new();
		AddChild(tempNavigationAgent);
		tempNavigationAgent.TargetPosition = targetPoint;
		bool isTargetReachable = tempNavigationAgent.IsTargetReachable();
		tempNavigationAgent.QueueFree();
		return isTargetReachable;
	}

	private void ProcessNavigation(double delta)
	{
		if (NavigationAgent == null) return;
		if (NavigationAgent.IsNavigationFinished()) return;

		Vector2 nextPosition = NavigationAgent.GetNextPathPosition();
		Vector2 direction = GlobalPosition.DirectionTo(nextPosition);
		// Vector2 newVelcity = direction * Speed;
		Vector2 newVelcity = Velocity.Lerp(direction * Speed, (float)(Acceleration * delta));

		if (NavigationAgent.AvoidanceEnabled)
		{
			NavigationAgent.Velocity = newVelcity;
		}
		else
		{
			OnSafeVelocityComputed(newVelcity);
		}

		MoveAndSlide();
	}

	public void OnSafeVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
	}

	public void TryInteractWith(ICanBeInteractedWith target)
	{
        if (target is Node2D targetNode2D)
        {
            if (!DetectionHandler.CanSee(targetNode2D)) return;

            target.InteractWith();
        }
	}

    public void TakeDamage(int damage)
    {
		GD.Print($"Taking {damage} damage");
		Stats.Health -= damage;
		if (Stats.Health <= 0)
		{
			GD.Print("Agent is dead");
			// QueueFree(); // TODO: Implement death logic
		}
    }

    public void ApplyVelocity(Vector2 velocity)
    {
		Vector2 oldVelocity = Velocity;

		Velocity += velocity;

		MoveAndSlide();

		Velocity = oldVelocity;
    }

	public Vector2 GetMoveableGlobalPosition()
	{
		return GlobalPosition;
	}

}
