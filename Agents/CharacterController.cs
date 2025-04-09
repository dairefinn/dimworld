namespace Dimworld.Agents;

using Dimworld.Developer;
using Dimworld.Dialogue;
using Dimworld.Effects;
using Dimworld.Helpers;
using Dimworld.Items;
using Dimworld.Memory;
using Dimworld.Memory.MemoryEntries;
using Dimworld.Modifiers;
using Godot;
using Godot.Collections;


// TODO: Might want to make this even more abstract and then have HumanoidController and AnimalController inherit from this (Because an animal might not have a speech bubble or clothing controller)
public partial class CharacterController : CharacterBody2D, IHasAgentStats, ICanBeMoved, IHasInventory, IMemorableNode, IAffectedByModifiers, ICanSpeak
{

	[ExportGroup("Movement")]
	[Export] public float Speed { get; set; } = 100f;
	[Export] public float Acceleration { get; set; } = 50f;
	[Export] public NavigationAgent2D NavigationAgent { get; set; }

	[ExportGroup("Inventory")]
	[Export] public Inventory Inventory { get; set; }
    [Export] public bool CanTakeFromInventory { get; set; } = false;

	[ExportGroup("Stats")]
	[Export] public AgentStats Stats {
		get => _stats;
		set {
			_stats = value;
			LinkStatsToUI();
		}
	}
	private AgentStats _stats;
	[Export] public AgentStatsUI StatsUI {
		get => _statsUI;
		set {
			_statsUI = value;
			LinkStatsToUI();
		}
	}
	private AgentStatsUI _statsUI;

	[ExportGroup("References")]
	[Export] public SpeechBubble SpeechBubble { get; set; }
	[Export] public ClothingController ClothingController { get; set; }
	[Export] public DetectionHandler DetectionHandler { get; set; }
	

	public ModifierHandler ModifierHandler { get; set; } = new();
	public MemoryHandler MemoryHandler { get; set; } = new();
	public EquipmentHandler EquipmentHandler { get; set; }


    private Vector2 desiredMovementDirection = Vector2.Zero;
	private Rid navigationRid;


	// LIFECYCLE EVENTS

	public override void _Ready()
	{
		base._Ready();

		EquipmentHandler = new EquipmentHandler(this);

		// Hack to get the AgentStats to be initialized as a separate instance
		if (Stats != null)
		{
			Stats = new AgentStats(Stats);
			Stats.HealthChanged += OnHealthChanged;
		}

		if (StatsUI == null)
		{
			StatsUI = GetNode<AgentStatsUI>("AgentStatsUI");
			StatsUI.Stats = Stats;
		}

		NavigationAgent.VelocityComputed += OnSafeVelocityComputed;

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
		base._Process(delta);

		ModifierHandler?.ProcessConditions(delta);
	}


	// STATS & STATS UI

	private void LinkStatsToUI()
	{
		if (StatsUI == null) return;
		StatsUI.Stats = Stats;
	}


	// DETECTION AND MEMORY

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
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

    public void OnHealthChanged()
    {
		if (Stats.Health > 0) return;
		DeveloperConsole.Print("Agent is dead");
		QueueFree(); // TODO: Implement a state machine and move the character to the dead state instead of deleting it.
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
