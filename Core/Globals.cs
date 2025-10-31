namespace Dimworld.Core;

using DaireFinn.Plugins.DeveloperConsole.UI;
using Dimworld.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Currency;
using Dimworld.Core.Developer;
using Dimworld.Core.Interaction;
using Dimworld.Core.Levels;
using Dimworld.Core.UI;
using Dimworld.Factions;
using Dimworld.UI.Dialogue;
using Dimworld.UI.Inventory;
using Godot;


// TODO: This should only reference the Core classes and not the other game-specfic scenes and classes (Make each of the game-specific nodes below an interface).
// TODO: Otherwise - it should be moved into the Game folder and note be referenced from the Core classes.
/// <summary>
/// This class contains references to all nodes which would otherwise be singletons.
/// This makes it so that they can actually be swapped out and ensures a single source of truth for each by ensuring that this is the only singleton.
/// The only exception to this is the DeveloperConsole, which is a singleton purely to make it easier to reference when logging.
/// </summary>
public partial class Globals : Node
{

    public static Globals Instance { get; private set; }


    public Globals()
    {
        if (Instance != null)
        {
            throw new System.Exception("Globals is a singleton and cannot be instantiated multiple times.");
        }

        Instance = this;
        MainThreadId = System.Environment.CurrentManagedThreadId;
    }


    public override void _Ready()
    {
        base._Ready();

        FactionDefaults.Initialize();
        DeveloperConsoleCommandHandler.RegisterCommands();
    }



    [Export]
    public Player Player
    {
        get => _player;
        set
        {
            _player = value;
            LinkPlayerInventory();
        }
    }
    private Player _player;

    [Export]
    public InventoryViewer InventoryViewer
    {
        get => _inventoryViewer;
        set
        {
            _inventoryViewer = value;
            LinkPlayerInventory();
        }
    }
    private InventoryViewer _inventoryViewer;

    [Export] public CursorFollower CursorFollower { get; set; }

    [Export] public LevelHandler LevelHandler { get; set; }

    [Export] public DayNightController DayNightController { get; set; }

    [Export] public DialoguePanelUI DialoguePanelUI { get; set; }

    [Export] public TradeController TradeController { get; set; }

    [Export] public CurrencyController CurrencyController { get; set; }

    [Export] public UIRootPanel UIRoot { get; set; }

    [Export] public DeveloperMenuUI DeveloperMenu { get; set; }


    public int MainThreadId { get; private set; }


    public void LinkPlayerInventory()
    {
        if (!IsInstanceValid(Player)) return;
        if (!IsInstanceValid(InventoryViewer)) return;

        InventoryViewer.PrimaryInventory = Player.Inventory;
    }

}
