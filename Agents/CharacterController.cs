namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class CharacterController : CharacterBody2D, IDamageable, ICanBeMoved, IAffectedByConditions, IGoapAgent, IHasInventory
{

	[ExportGroup("Properties")]
	[Export] public float Speed { get; set; } = 50f;
	[Export] public float Acceleration { get; set; } = 25f;
	[Export] public Inventory Inventory { get; set; }
	[Export] public AgentStats Stats {
		get => _stats;
		set {
			_stats = value;
			LinkStatsToUI();
		}
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
	[Export] public EquipmentHandler EquipmentHandler { get; set; }
	[Export] public AgentStatsUI StatsUI {
		get => _statsUI;
		set {
			_statsUI = value;
			LinkStatsToUI();
		}
	}
	private AgentStatsUI _statsUI;
	[Export] public SpeechBubble SpeechBubble { get; set; }
	[Export] public ConditionHandler ConditionHandler { get; set; }
	[Export] public PlanningHandler PlanningHandler { get; set; }
	[Export] public MemoryHandler MemoryHandler { get; set; }


	private Vector2 desiredMovementDirection = Vector2.Zero;


	// LIFECYCLE EVENTS

	public override void _Ready()
	{
		base._Ready();

		// Hack to get the AgentStats to be initialized as a separate instance
		if (Stats != null)
		{
			Stats = new AgentStats(Stats);
		}

		if (StatsUI == null)
		{
			StatsUI = GetNode<AgentStatsUI>("AgentStatsUI");
			StatsUI.Stats = Stats;
		}

		NavigationAgent.VelocityComputed += OnSafeVelocityComputed;

		DetectionHandler.OnNodeDetected += OnDetectionHandlerNodeDetected;

		SetInventoryState();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		ProcessNavigation(delta);

		ConditionHandler?.PhysicsProcessConditions(this, delta);
	}

	public override void _Process(double delta)
	{
		Inventory.OnUpdated += () => SetInventoryState();

		if (IsPlanningEnabled)
		{
			PlanningHandler?.OnProcess(this, delta);
		}

		ConditionHandler?.ProcessConditions(this, delta);

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


	// STATS & STATS UI

	private void LinkStatsToUI()
	{
		if (StatsUI == null) return;
		StatsUI.Stats = Stats;
	}


	// DETECTION AND MEMORY

	private void OnDetectionHandlerNodeDetected(Node node)
	{
		if (!IsPlanningEnabled) return;
		MemoryHandler?.OnNodeDetected(node);
	}

	// TODO: Use memory handler for this or check the characters inventory in the procedural conditions
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
		}

		WorldState.Remove("has_items");
		WorldState["has_items"] = itemsInInventory;
	}

	
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
		if (desiredMovementDirection != Vector2.Zero)
		{
			ProcessNavigationInput(desiredMovementDirection, delta);
		}
		else if (NavigationAgent.IsTargetReached())
		{
			Velocity = Velocity.Lerp(Vector2.Zero, (float)(Acceleration * delta));
		}
		else
		{
			ProcessNavigationPathfinding(delta);
		}

		MoveAndSlide();

		desiredMovementDirection = Vector2.Zero;
	}

	private void ProcessNavigationInput(Vector2 desiredMovementDirection, double delta)
	{
		Velocity = Velocity.Lerp(desiredMovementDirection * Speed, (float)(Acceleration * delta));
	}

	private void ProcessNavigationPathfinding(double delta)
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

	}

	public void OnSafeVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
	}

	public void SetMovementDirection(Vector2 direction)
	{
		if (direction == Vector2.Zero) return;
		desiredMovementDirection = direction;
	}


	// INTERACTION

	public void TryInteractWith(ICanBeInteractedWith target)
	{
        if (target is Node2D targetNode2D)
        {
            if (!DetectionHandler.CanSee(targetNode2D)) return;

            target.InteractWith();
        }
	}


	// DAMAGE HANDLING

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


	// FORCE APPLICATION

    public void ApplyVelocity(Vector2 newVelocity, double delta)
    {
		Vector2 oldVelocity = Velocity;

		Velocity = Velocity.Lerp(newVelocity, (float)(Acceleration * delta));

		MoveAndSlide();

		Velocity = oldVelocity;
    }

	public Vector2 GetMoveableGlobalPosition()
	{
		return GlobalPosition;
	}

	// CONDITIONS

	public IConditionHandler GetConditionHandler()
	{
		return ConditionHandler;
	}


	// SPEECH

	public void Say(string text)
	{
		SpeechBubble?.Say(text);
	}

}
