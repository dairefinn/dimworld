namespace Dimworld.MemoryEntries;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class InventoryContents : MemoryEntry
{

    public IHasInventory Node { get; set; } = null;
    public Array<InventorySlot> InventorySlots { get; set; } = [];


    public override bool IsRelatedToNode(Node node)
    {
        if (Node == null) return false;
        return node == Node;
    }

    public override string ToString()
    {
        if (Node != null)
        {
            return $"{Node.GetType()} [{string.Join(", ", InventorySlots.Select(item => $"{item.Item.ItemName} x {item.Quantity}"))}]";
        }

        return base.ToString();
    }

    public override  MemoryEntry GetMatchingEntryFrom(Array<MemoryEntry> memoryEntries)
    {
        foreach (MemoryEntry entry in memoryEntries)
        {
            if (entry is NodeLocation nodeLocationEntry)
            {
                if (nodeLocationEntry.Node == Node)
                {
                    return nodeLocationEntry;
                }
            }
        }

        return null;
    }

    public override bool Equals(MemoryEntry other)
    {
        if (other is InventoryContents otherNodeLocation)
        {
            return Node == otherNodeLocation.Node && InventorySlots.SequenceEqual(otherNodeLocation.InventorySlots);
        }

        return false;
    }


    public static InventoryContents FromNode(IHasInventory nodeWithInventory)
    {
        if (nodeWithInventory == null) return null;
        if (nodeWithInventory.Inventory == null) return null;

        InventoryContents chestContents = new()
        {
            Node = nodeWithInventory,
            InventorySlots = [..nodeWithInventory.Inventory.Slots.Where(slot => !slot.IsEmpty).ToArray()]
        };

        return chestContents;
    }

}
