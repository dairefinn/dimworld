namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    [Export] public Inventory Inventory;


    // TODO: This currently just adds a sword to your inventory but it should open a screen where you can compare inventories
    public void InteractWith()
    {
        Globals.GetInstance().MainPlayer.InventoryHandler.OpenSecondaryInventory(Inventory);
    }

}
