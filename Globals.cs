namespace Dimworld;

using Dimworld.Items.UI;
using Dimworld.Levels;
using Godot;


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


    [Export] public CharacterController Player {
        get => _player;
        set {
            _player = value;
            LinkPlayerInventory();
        }
    }
    private CharacterController _player;

    [Export] public InventoryViewer InventoryViewer {
        get => _inventoryViewer;
        set {
            _inventoryViewer = value;
            LinkPlayerInventory();
        }
    }
    private InventoryViewer _inventoryViewer;

    [Export] public CursorFollower CursorFollower { get; set; }

    [Export] public LevelHandler LevelHandler { get; set; }

    [Export] public DayNightController DayNightController { get; set; }


    public int MainThreadId { get; private set; }


    public void LinkPlayerInventory()
    {
        if (!IsInstanceValid(Player)) return;
        if (!IsInstanceValid(InventoryViewer)) return;

        InventoryViewer.PrimaryInventory = Player.Inventory;
    }

}
