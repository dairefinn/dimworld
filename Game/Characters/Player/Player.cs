namespace Dimworld.Characters;

using Dimworld.Agents.Instances.States;
using Dimworld.Core;
using Dimworld.Core.StateMachines;
using Dimworld.Core.Items;
using Dimworld.Core.Levels.Transitions;
using Dimworld.UI.Inventory;
using Godot;
using Godot.Collections;
using Dimworld.Core.Developer;
using Dimworld.Core.Characters;
using Dimworld.Core.Modifiers;

public partial class Player : CharacterController, ICanTriggerLevelTransitions
{

	public enum States
	{
		Idle,
		Walking,
		Running,
		Attacking,
		Interacting,
		Dead
	}


    private static bool CanUseInputs => !Globals.Instance.DeveloperMenu.Visible && !DeveloperConsole.IsFocused && !(Globals.Instance.Player.Stats.Health <= 0);


	/// <summary>
	/// This enables pathfinding for the player. If we need to move them automatically (e.g. a cutscene), we can set this to true.
	/// </summary>
	public bool UsePathfinding { get; set; } = false;
	public Vector2 DesiredMovementDirection { get; set; } = Vector2.Zero;


    public bool TryingToAttack => 
            Input.IsActionPressed(InputActions.LEFT_MOUSE) &&
            !Globals.Instance.InventoryViewer.IsViewing &&
            Globals.Instance.InventoryViewer.Hotbar.SelectedSlotUI != null &&
            !Globals.Instance.InventoryViewer.Hotbar.SelectedSlotUI.TargetSlot.IsEmpty &&
            Globals.Instance.InventoryViewer.Hotbar.SelectedSlotUI.TargetSlot.Item != null &&
            Globals.Instance.InventoryViewer.Hotbar.SelectedSlotUI.TargetSlot.Item is ICanBeUsedFromHotbar;
    public bool TryingToInteract =>
        Input.IsActionPressed(InputActions.INTERACT) &&
        !Globals.Instance.InventoryViewer.IsViewing;


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
			Input.IsActionPressed(InputActions.MOVE_RIGHT) ? 1 : (Input.IsActionPressed(InputActions.MOVE_LEFT) ? -1 : 0),
			Input.IsActionPressed(InputActions.MOVE_DOWN) ? 1 : (Input.IsActionPressed(InputActions.MOVE_UP) ? -1 : 0)
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

        bool isTogglingDeveloperMenu = @event.IsActionPressed(InputActions.TOGGLE_DEVELOPER_MENU);

        bool canOpenInventory = CanUseInputs && @event.IsActionPressed(InputActions.TOGGLE_INVENTORY) && !inventoryViewer.IsViewing;
        bool canCloseInventory = CanUseInputs && (@event.IsActionPressed(InputActions.TOGGLE_INVENTORY) || @event.IsActionPressed(InputActions.INTERACT) || @event.IsActionPressed(InputActions.UI_CANCEL)) && inventoryViewer.IsViewing;
        bool canCloseConsole = @event.IsActionPressed(InputActions.UI_CANCEL) && DeveloperConsole.IsFocused;
        bool canReload = CanUseInputs && @event.IsActionPressed(InputActions.ACTION_RELOAD) && !inventoryViewer.IsViewing;

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
            Globals.Instance.DeveloperMenu.ToggleVisibility();
        }

        if (canCloseConsole)
        {
            Globals.Instance.DeveloperMenu.Hide();
        }

        if (canReload)
        {
            inventoryViewer.TryReloadSelectedItem();
        }

        if (CanUseInputs)
        {
            // If 1-5 are pressed, use the corresponding hotbar item
            if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_0))
            {
                inventoryViewer.Hotbar.SelectSlot(0);
            }
            else if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_1))
            {
                inventoryViewer.Hotbar.SelectSlot(1);
            }
            else if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_2))
            {
                inventoryViewer.Hotbar.SelectSlot(2);
            }
            else if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_3))
            {
                inventoryViewer.Hotbar.SelectSlot(3);
            }
            else if (@event.IsActionPressed(InputActions.HOTBAR_SLOT_4))
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
