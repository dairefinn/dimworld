namespace Dimworld.Memory;

using System.Linq;
using Dimworld.Developer;
using Dimworld.Helpers;
using Godot;
using Godot.Collections;


public partial class MemoryHandler : Node2D
{

    [Signal] public delegate void OnMemoryEntryAddedEventHandler(MemoryEntry memoryEntry);
    [Signal] public delegate void OnMemoryEntryRemovedEventHandler(MemoryEntry memoryEntry);


    public Array<MemoryEntry> MemoryEntries { get; set; } = [];


    public override void _Ready()
    {
        OnMemoryEntryAdded += (entry) => {
            DeveloperConsole.Print($"Entry added to agent memory: {BBCodeHelper.Colors.Yellow(Json.Stringify(entry))}");
            LogCurrentMemoryEntries();
        };
        OnMemoryEntryRemoved += (entry) => {
            DeveloperConsole.Print($"Entry removed from agent memory: {BBCodeHelper.Colors.Yellow(Json.Stringify(entry))}");
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
                DeveloperConsole.Print($"Memory expired: {Json.Stringify(memoryEntry)}");
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
        // DeveloperConsole.Print("Current Memory Entries: "+ memoryEntriesString);
    }

}
