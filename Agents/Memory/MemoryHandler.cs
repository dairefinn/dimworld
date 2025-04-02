namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


// TODO: Agent should forget (some) memories over time
public partial class MemoryHandler : Node2D
{

    [Signal] public delegate void OnMemoryEntryAddedEventHandler(MemoryEntry memoryEntry);

    [Export] public Array<MemoryEntry> MemoryEntries { get; set; } = [];


    public override void _Ready()
    {
        OnMemoryEntryAdded += (entry) => {
            GD.Print($"Entry added to agent memory: {Json.Stringify(entry)}");
            LogCurrentMemoryEntries();
        };
    }


    public void OnNodeDetected(Node node)
    {
        if (node is IMemorableNode memorableNode)
        {
            AddMemory(memorableNode.GetNodeLocationMemory());
        }

        // Done manually when interacting with the node
        // if (node is IHasInventory nodeWithInventory)
        // {
        //     AddMemory(InventoryContents.FromNode(nodeWithInventory));
        // }
    }

    public bool AddMemory(MemoryEntry memoryEntry)
    {
        if (memoryEntry == null) return false; // Invalid memory entry

        // Remove the existing memory entry if it matches the new one. This methods keeps the matching logic on the different memory entry types
        MemoryEntry matchingEntry = memoryEntry.GetMatchingEntryFrom(MemoryEntries);
        if (matchingEntry != null)
        {
            if (matchingEntry.Equals(memoryEntry)) return false; // If they're the same, do nothing
            MemoryEntries.Remove(matchingEntry);
        }

        MemoryEntries.Add(memoryEntry);
        EmitSignal(SignalName.OnMemoryEntryAdded, memoryEntry);

        return true;
    }

    private void LogCurrentMemoryEntries()
    {
        // JSON serialization for better readability
        // string memoryEntriesString = Json.Stringify(MemoryEntries.Select(entry => entry.ToString()).ToArray());
        // GD.Print("Current Memory Entries: "+ memoryEntriesString);
    }

}
