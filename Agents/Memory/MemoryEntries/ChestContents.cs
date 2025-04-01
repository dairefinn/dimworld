namespace Dimworld.MemoryEntries;

using Godot;
using Godot.Collections;

public partial class ChestContents : MemoryEntry
{

    public Chest AssociatedChest { get; set; } = null;
    public Array<InventoryItem> InventoryItems { get; set; } = [];

}
