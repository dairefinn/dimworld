namespace Dimworld;

using Godot;


public partial class PlayerController : Node
{

    [Export] public bool IsCurrentPlayer { get; set; } = false;
    [Export] public InventoryHandler InventoryHandler { get; set; }
    [Export] public PlayerInteractionHandler InteractionHandler { get; set; }
    [Export] public AgentMovementController MovementController { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (IsCurrentPlayer && Globals.GetInstance().MainPlayer == null)
        {
            Globals.GetInstance().MainPlayer = this;
        }
    }

}
