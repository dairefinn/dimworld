namespace Dimworld.Core.Characters.Memory;

using System.Linq;
using Dimworld.Core.Developer;
using Dimworld.Core.Utils;
using Godot;
using Godot.Collections;


/// <summary>
/// Manages any memories an entity has.
/// </summary>
public partial class MemoryHandler : Node
{

    [Signal] public delegate void OnMemoryEntryAddedEventHandler(MemoryEntry memoryEntry);
    [Signal] public delegate void OnMemoryEntryRemovedEventHandler(MemoryEntry memoryEntry);


    public Array<MemoryEntry> MemoryEntries { get; set; } = [];


    private void LogCurrentMemoryEntries()
    {
        // JSON serialization for better readability
        // string memoryEntriesString = Json.Stringify(MemoryEntries.Select(entry => entry.ToString()).ToArray());
        // DeveloperConsole.Print("Current Memory Entries: "+ memoryEntriesString);
    }


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


    /// <summary>
    /// Called when a node is detected. Determines if a memory entry should be added based on the node type.
    /// </summary>
    /// <param name="node">The detected node.</param>
    public void OnNodeDetected(Node node)
    {
        if (node is IMemorableNode memorableNode)
        {
            AddMemory(memorableNode.GetNodeLocationMemory());
        }
    }

    /// <summary>
    /// Adds a memory entry to the memory list. If a matching entry already exists, it is removed first.
    /// </summary>
    /// <param name="memoryEntry">The memory entry to add.</param>
    /// <returns>True if the memory entry was added successfully; otherwise, false.</returns>
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

    /// <summary>
    /// Removes a memory entry from the memory list. If the entry is not found, it returns false.
    /// </summary>
    /// <param name="memoryEntry">The memory entry to remove.</param>
    /// <returns>True if the memory entry was removed successfully; otherwise, false.</returns>
    public bool RemoveMemory(MemoryEntry memoryEntry)
    {
        if (memoryEntry == null) return false; // Invalid memory entry
        if (!MemoryEntries.Contains(memoryEntry)) return false; // Memory entry not found
        MemoryEntries.Remove(memoryEntry);
        EmitSignal(SignalName.OnMemoryEntryRemoved, memoryEntry);
        return true;
    }

    /// <summary>
    /// Retrieves all memory entries of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of memory entry to retrieve.</typeparam>
    /// <returns>An array of memory entries of the specified type.</returns>
    public T[] GetMemoriesOfType<T>() where T : MemoryEntry
    {
        return MemoryEntries.OfType<T>().ToArray();
    }

}
