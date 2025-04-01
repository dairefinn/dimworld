namespace Dimworld.MemoryEntries;

using System.Linq;
using Godot.Collections;

public partial class InventoryContents : MemoryEntry
{

    public IHasInventory AssociatedChest { get; set; } = null;
    public Array<InventorySlot> InventorySlots { get; set; } = [];

    public override string ToString()
    {
        if (AssociatedChest != null)
        {
            return $"{AssociatedChest} [{string.Join(", ", InventorySlots.Select(item => $"{item.Item.ItemName} x {item.Quantity}"))}]";
        }

        return base.ToString();
    }


    public static InventoryContents FromNode(IHasInventory nodeWithInventory)
    {
        if (nodeWithInventory == null) return null;
        if (nodeWithInventory.Inventory == null) return null;

        InventoryContents chestContents = new()
        {
            AssociatedChest = nodeWithInventory,
            InventorySlots = [..nodeWithInventory.Inventory.Slots.Where(slot => !slot.IsEmpty).ToArray()]
        };

        return chestContents;
    }

}
