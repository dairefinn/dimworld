namespace Dimworld.Agents.Instances;

using Dimworld.Agents.Instances.States;
using Dimworld.Developer;
using Dimworld.Items.UI;
using Dimworld.Levels;
using Dimworld.Modifiers;
using Dimworld.States;
using Godot;
using Godot.Collections;


public partial class Player : CharacterController, ICanTriggerLevelTransitions
{

    private static bool CanUseInputs => !DeveloperMenu.IsOpen && !DeveloperConsole.IsFocused && !(Globals.Instance.Player.Stats.Health <= 0);


	/// <summary>
	/// This enables pathfinding for the player. If we need to move them automatically (e.g. a cutscene), we can set this to true.
	/// </summary>
	public bool UsePathfinding { get; set; } = false;
	public Vector2 DesiredMovementDirection { get; set; } = Vector2.Zero;


	private StateMachine<Player> _stateMachine;
	private Dictionary<string, State<Player>> _states = new()
	{
		{ States.Idle.ToString(), new PlayerIdleState() },
		{ States.Walking.ToString(), new PlayerWalkingState() },
		{ States.Running.ToString(), new PlayerRunningState() },
		{ States.Attacking.ToString(), new PlayerAttackingState() },
		{ States.Interacting.ToString(), new PlayerInteractingState() },
		{ States.Dead.ToString(), new PlayerDeadState() }
	};


    public Player()
    {
        Globals.Instance.Player = this;
    }


	private Vector2 GetDesiredMovementDirection()
	{
		Vector2 direction = new(
			Input.IsActionPressed("move_right") ? 1 : (Input.IsActionPressed("move_left") ? -1 : 0),
			Input.IsActionPressed("move_down") ? 1 : (Input.IsActionPressed("move_up") ? -1 : 0)
		);

		return direction;
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

    private void TryInteract(CursorFollower cursorFollower)
    {
        if (!IsInstanceValid(cursorFollower)) return;

        ICanBeInteractedWith interactableObject = cursorFollower.InteractableObject;
        if (interactableObject == null) return;

        TryInteractWith(interactableObject);
    }


	public override void _Ready()
	{
		base._Ready();

        _stateMachine = new StateMachine<Player>(this, _states, States.Idle.ToString());
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

        DesiredMovementDirection = GetDesiredMovementDirection();

		_stateMachine?.OnProcess(delta);
    }

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		_stateMachine?.OnPhysicsProcess(delta);
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        InventoryViewer inventoryViewer = Globals.Instance.InventoryViewer;
        CursorFollower cursorFollower = Globals.Instance.CursorFollower;

        bool isTogglingDeveloperMenu = @event.IsActionPressed("toggle_developer_menu");

        bool canInteract = CanUseInputs && @event.IsActionPressed("interact") && !inventoryViewer.IsViewing;
        bool canOpenInventory = CanUseInputs && @event.IsActionPressed("toggle_inventory") && !inventoryViewer.IsViewing;
        bool canCloseInventory = CanUseInputs && (@event.IsActionPressed("toggle_inventory") || @event.IsActionPressed("interact") || @event.IsActionPressed("ui_cancel")) && inventoryViewer.IsViewing;
        bool canUseHotbarItems = CanUseInputs && !inventoryViewer.IsViewing && @event.IsActionPressed("lmb");
        bool canCloseConsole = @event.IsActionPressed("ui_cancel") && DeveloperConsole.IsFocused;
        bool canReload = CanUseInputs && @event.IsActionPressed("action_reload") && !inventoryViewer.IsViewing;
        

        if (canInteract)
        {
            TryInteract(cursorFollower);
        }

        if (canOpenInventory)
        {
            inventoryViewer.SetPrimaryInventoryVisibility(true);
        }

        if (canCloseInventory)
        {
            inventoryViewer.SetBothInventoriesVisibility(false);
        }

        if (isTogglingDeveloperMenu)
        {
            DeveloperMenu.Instance?.ToggleVisibility();
        }

        if (canCloseConsole)
        {
            DeveloperMenu.Instance?.Hide();
        }

        if (canUseHotbarItems)
        {
            inventoryViewer.TryUseSelectedItem();
        }

        if (canReload)
        {
            inventoryViewer.TryReloadSelectedItem();
        }

        if (CanUseInputs)
        {
            // If 1-5 are pressed, use the corresponding hotbar item
            if (@event.IsActionPressed("hotbar_slot_0"))
            {
                inventoryViewer.Hotbar.SelectSlot(0);
            }
            else if (@event.IsActionPressed("hotbar_slot_1"))
            {
                inventoryViewer.Hotbar.SelectSlot(1);
            }
            else if (@event.IsActionPressed("hotbar_slot_2"))
            {
                inventoryViewer.Hotbar.SelectSlot(2);
            }
            else if (@event.IsActionPressed("hotbar_slot_3"))
            {
                inventoryViewer.Hotbar.SelectSlot(3);
            }
            else if (@event.IsActionPressed("hotbar_slot_4"))
            {
                inventoryViewer.Hotbar.SelectSlot(4);
            }
        }
    }


    public override void OnDeath()
    {
		_stateMachine.TransitionTo(States.Dead.ToString());
    }


    protected override void ProcessNavigation(double delta)
    {
		if (UsePathfinding)
		{
			base.ProcessNavigation(delta);
			return;
		}

		ProcessNavigationInput(DesiredMovementDirection, delta);
		MoveAndSlide();
    }

}
