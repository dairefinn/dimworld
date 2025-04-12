namespace Dimworld.Core.Characters;

using Dimworld.Core.GOAP;
using Dimworld.Core.Items;
using Godot;
using Godot.Collections;


/// <summary>
/// An implementation of the CharacterController with some NPC-specific functionality added.
/// </summary>
public partial class NpcController : CharacterController, IGoapAgent
{

	[ExportGroup("Planning")]
	[Export] public bool IsPlanningEnabled { get; set; } = true;
	[Export] public GoapState WorldState { get; set; } = new();
	[Export] public Array<GoapAction> ActionSet { get; set; } = [];
	[Export] public Array<GoapGoal> GoalSet { get; set; } = [];


	public Vector2 GlobalPositionThreadSafe { get; set; } = Vector2.Zero;
	public PlanningHandler PlanningHandler { get; set; } = new();


    public override void _Ready()
    {
        base._Ready();
        
		DetectionHandler.OnNodeDetected += OnDetectionHandlerNodeDetected;

		SetInventoryState();
    }
    
	public override void _Process(double delta)
	{
        base._Process(delta);

		GlobalPositionThreadSafe = GlobalPosition;

		Inventory.OnUpdated += () => SetInventoryState();

		if (IsPlanningEnabled)
		{
			PlanningHandler?.OnProcess(this, delta);
		}

		ModifierHandler?.ProcessConditions(delta);
	}


	private void OnDetectionHandlerNodeDetected(Node node)
	{
		if (!IsPlanningEnabled) return;
		MemoryHandler?.OnNodeDetected(node);
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

}
