namespace Dimworld;

using System.Linq;
using Dimworld.MemoryEntries;
using Godot;

public partial class Chest : StaticBody2D, ICanBeInteractedWith, IMemorableNode, IHasInventory
{

    [Export] public Inventory Inventory { get; set; }


    public void InteractWith()
    {
        InputHandler.Instance.InventoryViewer.OpenSecondaryInventory(Inventory);
    }

    public bool ContainsItem(InventoryItem item)
    {
        return Inventory.HasItem(item);
    }

    public bool ContainsItem(string itemId)
    {
        return Inventory.HasItem(itemId);
    }

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
    }

}
