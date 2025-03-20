namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    [Export] public Inventory Inventory;


    public void InteractWith()
    {
        Globals.GetInstance().MainPlayer.InventoryHandler.OpenSecondaryInventory(Inventory);
    }

}
