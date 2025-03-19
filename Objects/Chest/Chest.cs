namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    // TODO: This currently just adds a sword to your inventory but it should open a screen where you can compare inventories
    public void InteractWith()
    {
        GD.Print("Interacting with chest");

        InventoryItem itemDuplicate = GD.Load("res://Items/Sword.tres").Duplicate() as InventoryItem;
        bool success = Globals.GetInstance().MainPlayer.Inventory.AddItem(itemDuplicate);
        if (success)
        {
            GD.Print("Added item to inventory: " + itemDuplicate.ItemName);
        }
        else
        {
            GD.Print("Failed to add item to inventory: " + itemDuplicate.ItemName);
        }
    }

}
