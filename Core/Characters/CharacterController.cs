namespace Dimworld.Core.Characters;

using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Characters.Stats;
using Dimworld.Core.Developer;
using Dimworld.Core.Effects;
using Dimworld.Core.Interaction;
using Dimworld.Core.Items;
using Dimworld.Core.Modifiers;
using Dimworld.Core.Utils;
using Dimworld.UI.Characters;
using Godot;


/// <summary>
/// Base class for all character controllers.
/// This class handles the common functionality for all character controllers.
/// </summary>
public partial class CharacterController : CharacterBody2D, IHasCharacterStats, ICanBeMoved, IHasInventory, IMemorableNode, IAffectedByModifiers, ICanSpeak, IHasNavigation, IHasMemory
{

	[ExportGroup("Movement")]
	[Export] public float Speed { get; set; } = 100f;
	[Export] public float Acceleration { get; set; } = 50f;
	[Export] public NavigationAgent2D NavigationAgent { get; set; }

	[ExportGroup("Inventory")]
	[Export] public Inventory Inventory { get; set; }
    [Export] public bool CanTakeFromInventory { get; set; } = false;

	[ExportGroup("Stats")]
	[Export] public CharacterStats Stats {
		get => _stats;
		set {
			_stats = value;
			LinkStatsToUI();
		}
	}
	private CharacterStats _stats;
	[Export] public CharacterStatsUI StatsUI { // TODO: StatsUI Should be given a reference to a node with IHasCharacterStats
		get => _statsUI;
		set {
			_statsUI = value;
			LinkStatsToUI();
		}
	}
	private CharacterStatsUI _statsUI;

	[ExportGroup("References")]
	[Export] public ClothingController ClothingController { get; set; }
	[Export] public DetectionHandler DetectionHandler { get; set; }
	[Export] public SpeechHandler SpeechHandler { get; set; }
	[Export] public AnimationPlayer AnimationPlayer { get; set; }
	

	public ModifierHandler ModifierHandler { get; set; } = new();
	public MemoryHandler MemoryHandler { get; set; } = new();
	public EquipmentHandler EquipmentHandler { get; set; }

    private Rid _navigationRid;


	// LIFECYCLE EVENTS

	public override void _Ready()
	{
		base._Ready();

		EquipmentHandler = new EquipmentHandler(this);

		// Hack to get the AgentStats to be initialized as a separate instance
		if (Stats != null)
		{
			Stats = new CharacterStats(Stats);
			Stats.HealthChanged += OnHealthChanged;
		}

		if (StatsUI == null)
		{
			StatsUI = GetNode<CharacterStatsUI>("AgentStatsUI");
			StatsUI.Stats = Stats;
		}

		if (SpeechHandler != null)
		{
			SpeechHandler.Initalize(this);
		}

		NavigationAgent.VelocityComputed += OnSafeVelocityComputed;

		_navigationRid = NavigationAgent.GetNavigationMap();
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

	public bool IsTargetReached()
	{
		return NavigationAgent.IsTargetReached();
	}

	public bool CanReachPoint(Vector2 targetPoint)
	{
		bool isTargetReachable = NavigationServer2D.MapGetClosestPoint(_navigationRid, targetPoint).IsEqualApprox(targetPoint);
		return isTargetReachable;
	}

	public bool IsTargetingPoint(Vector2 targetPoint)
	{
		return NavigationAgent.TargetPosition.IsEqualApprox(targetPoint);
	}

	public Vector2 GetTargetPosition()
	{
		return NavigationAgent.TargetPosition;
	}

	protected virtual void ProcessNavigation(double delta)
	{
		if (NavigationAgent.IsTargetReached())
		{
			Velocity = Velocity.Lerp(Vector2.Zero, (float)(Acceleration * delta));
		}
		else
		{
			ProcessNavigationPathfinding(delta);
		}

		MoveAndSlide();
	}

	protected void ProcessNavigationPathfinding(double delta)
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
		OnDeath();
    }

    public virtual void OnDeath()
    {
        QueueFree();
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
		SpeechHandler?.Say(text);
	}

}
