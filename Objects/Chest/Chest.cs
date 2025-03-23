namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    [Export] public Inventory Inventory;


    public void InteractWith()
    {
        Globals.GetInstance().MainPlayer.InventoryViewer.OpenSecondaryInventory(Inventory);
    }

    public bool ContainsItem(InventoryItem item)
    {
        return Inventory.HasItem(item);
    }

    public bool ContainsItem(string itemId)
    {
        return Inventory.HasItem(itemId);
    }

}
