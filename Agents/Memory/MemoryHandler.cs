namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


// TODO: Agent should forget (some) memories over time
public partial class MemoryHandler : Node2D
{

    [Signal] public delegate void OnMemoryEntryAddedEventHandler(MemoryEntry memoryEntry);
    [Signal] public delegate void OnMemoryEntryRemovedEventHandler(MemoryEntry memoryEntry);

    [Export] public Array<MemoryEntry> MemoryEntries { get; set; } = [];


    public override void _Ready()
    {
        OnMemoryEntryAdded += (entry) => {
            GD.Print($"Entry added to agent memory: {Json.Stringify(entry)}");
            LogCurrentMemoryEntries();
        };
        OnMemoryEntryRemoved += (entry) => {
            // GD.Print($"Entry removed from agent memory: {Json.Stringify(entry)}");
            LogCurrentMemoryEntries();
        };
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (MemoryEntry memoryEntry in MemoryEntries)
        {
            memoryEntry.OnProcess(delta);

            if (memoryEntry.IsExpired())
            {
                GD.Print($"Memory expired: {Json.Stringify(memoryEntry)}");
                CallDeferred(MethodName.RemoveMemory, memoryEntry);
            }
        }
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
            // if (matchingEntry.Equals(memoryEntry)) return false; // If they're the same, do nothing
            RemoveMemory(matchingEntry);
        }

        MemoryEntries.Add(memoryEntry);
        EmitSignal(SignalName.OnMemoryEntryAdded, memoryEntry);

        return true;
    }

    public bool RemoveMemory(MemoryEntry memoryEntry)
    {
        if (memoryEntry == null) return false; // Invalid memory entry
        if (!MemoryEntries.Contains(memoryEntry)) return false; // Memory entry not found
        MemoryEntries.Remove(memoryEntry);
        EmitSignal(SignalName.OnMemoryEntryRemoved, memoryEntry);
        return true;
    }

    public T[] GetMemoriesOfType<T>() where T : MemoryEntry
    {
        return MemoryEntries.OfType<T>().ToArray();
    }

    private void LogCurrentMemoryEntries()
    {
        // JSON serialization for better readability
        // string memoryEntriesString = Json.Stringify(MemoryEntries.Select(entry => entry.ToString()).ToArray());
        // GD.Print("Current Memory Entries: "+ memoryEntriesString);
    }

}
