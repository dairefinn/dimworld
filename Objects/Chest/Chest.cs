namespace Dimworld;

using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith
{

    public void InteractWith()
    {
        GD.Print("Interacting with chest");

        InventoryItem itemDuplicate = GD.Load("res://Items/TestItem.tres").Duplicate() as InventoryItem;
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
