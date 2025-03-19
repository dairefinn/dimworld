namespace Dimworld;

using Godot;


public partial class PlayerController : Node
{

    [Export] public bool IsCurrentPlayer { get; set; } = false;
    [Export] public Inventory Inventory { get; set; }
    [Export] public PlayerInteractionHandler InteractionHandler { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (IsCurrentPlayer && Globals.GetInstance().MainPlayer == null)
        {
            Globals.GetInstance().MainPlayer = this;
        }
    }

}
