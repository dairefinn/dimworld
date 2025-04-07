namespace Dimworld;

using Dimworld.Developer;
using Dimworld.Dialogue;
using Dimworld.Effects;
using Dimworld.GOAP;
using Dimworld.Helpers;
using Dimworld.Items;
using Dimworld.Memory;
using Dimworld.Memory.MemoryEntries;
using Dimworld.Modifiers;
using Godot;
using Godot.Collections;


public partial class CharacterController : CharacterBody2D, IHasAgentStats, ICanBeMoved, IGoapAgent, IHasInventory, IMemorableNode, IAffectedByModifiers
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
    [Export] public bool CanTakeFromInventory { get; set; } = false;

	[ExportGroup("GOAP properties")]
	[Export] public bool IsPlanningEnabled { get; set; } = true;
	[Export] public GoapState WorldState { get; set; }
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
	[Export] public PlanningHandler PlanningHandler { get; set; }
	[Export] public MemoryHandler MemoryHandler { get; set; }
	[Export] public ModifierHandler ModifierHandler { get; set; } = new();
	[Export] public ClothingController ClothingController { get; set; }
	

	public Vector2 GlobalPositionThreadSafe { get; set; }


    private Vector2 desiredMovementDirection = Vector2.Zero;
	private Rid navigationRid;


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

		navigationRid = NavigationAgent.GetNavigationMap();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		ProcessNavigation(delta);

		ModifierHandler?.PhysicsProcessConditions(delta);
	}

	public override void _Process(double delta)
	{
		GlobalPositionThreadSafe = GlobalPosition;

		Inventory.OnUpdated += () => SetInventoryState();

		if (IsPlanningEnabled)
		{
			PlanningHandler?.OnProcess(this, delta);
		}

		ModifierHandler?.ProcessConditions(delta);
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

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
    }

	// TODO: Add an interface called IAffectsState that adds data dynamically to the agent's world state under the given key of 'has_items'. Can do the same for equipment with the key 'has_equipped'
	public void SetInventoryState()
	{
		if (WorldState == null)
		{
			WorldState = GoapState.Empty;
		}

		Array<string> itemsInInventory = [];

		foreach (InventorySlot slot in Inventory.Slots)
		{
			if (slot.IsEmpty) continue;
			itemsInInventory.Add(slot.Item.Id);
		}

		WorldState.RemoveKey("has_items");
		WorldState.Set("has_items", itemsInInventory);
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
		bool isTargetReachable = NavigationServer2D.MapGetClosestPoint(navigationRid, targetPoint).IsEqualApprox(targetPoint);
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
		Vector2 desiredVelocity = desiredMovementDirection * Speed;

		// Apply velocity modifiers
		Array<VelocityModifier> velocityModifiers = ModifierHandler.GetAllByType<VelocityModifier>();
		foreach (VelocityModifier velocityModifier in velocityModifiers)
		{
			desiredVelocity = velocityModifier.ApplyTo(desiredVelocity);
		}

		Velocity = Velocity.Lerp(desiredVelocity, (float)(Acceleration * delta));
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

	public void SetSprinting(bool isSprinting)
	{
		if (isSprinting)
		{
			ModifierHandler.Add(new VelocityMultiplyModifier("character_sprint", 2f));
		}
		else
		{
			ModifierHandler.RemoveByKey("character_sprint");
		}
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
		DeveloperConsole.Print($"Taking {damage} damage");
		Stats.Health -= damage;
		if (Stats.Health <= 0)
		{
			DeveloperConsole.Print("Agent is dead");
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


	// SPEECH

	public void Say(string text)
	{
		string formattedText = $"{BBCodeHelper.Colors.Cyan(text)}";
		DeveloperConsole.Print($"Guard: \"{formattedText}\"");
		SpeechBubble?.Say(text);
	}

}
