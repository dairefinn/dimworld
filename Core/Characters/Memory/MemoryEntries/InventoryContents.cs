namespace Dimworld.Core.Characters.Memory.MemoryEntries;

using System.Linq;
using Dimworld.Core.Items;
using Godot;
using Godot.Collections;


/// <summary>
/// Class representing a memory of what the contents of an inventory were.
/// This should be given to an agent whenever they interact with an inventory so that they can remember what was in it.
/// </summary>
public partial class InventoryContents : MemoryEntry
{

    public IHasInventory Node { get; set; } = null;
    public Inventory Inventory { get; set; }


    public InventoryContents()
    {
    }

    public InventoryContents(IHasInventory nodeWithInventory)
    {
        Node = nodeWithInventory; // TODO: Might want to store an ID here instead - if the node moves in the world this reference might give them knowledge of it when they shouldn't
        Inventory = nodeWithInventory.Inventory.Duplicate(true) as Inventory;
    }


    public override bool IsRelatedToNode(Node node)
    {
        if (Node == null) return false;
        return node == Node;
    }

    public override string ToString()
    {
        if (Node != null && Inventory != null)
        {
            return $"Inventory contents: {Node.GetType()} "
                 + $"[{string.Join(", ", Inventory.Slots
                        .Where(slot => !slot.IsEmpty)
                        .Select(slot => $"{slot.Item.ItemName} x {slot.Quantity}"
                    ))}]";
        }

        return base.ToString();
    }

    public override MemoryEntry GetMatchingEntryFrom(Array<MemoryEntry> memoryEntries)
    {
        foreach (MemoryEntry entry in memoryEntries)
        {
            if (entry is InventoryContents inventoryContentsEntry)
            {
                if (inventoryContentsEntry.Node == Node)
                {
                    return inventoryContentsEntry;
                }
            }
        }

        return null;
    }

    public override bool Equals(MemoryEntry other)
    {
        if (other is not InventoryContents otherNodeLocation) return false;
        if (Node == null || otherNodeLocation.Node == null) return false;
        
        foreach (InventorySlot slot in Inventory.Slots)
        {
            if (slot.IsEmpty) continue; // Skip empty slots TODO: IS this method needed?
            
        }

        return false;
    }

}
