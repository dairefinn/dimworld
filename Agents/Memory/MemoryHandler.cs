namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


public partial class MemoryHandler : Node2D
{

    [Signal] public delegate void OnMemoryEntryAddedEventHandler(MemoryEntry memoryEntry);

    [Export] public Array<MemoryEntry> MemoryEntries { get; set; } = [];


    public override void _Ready()
    {
        OnMemoryEntryAdded += (entry) => {
            LogCurrentMemoryEntries();
        };
    }


    public void OnNodeDetected(Node node)
    {
        GD.Print($"Node detected: {node.Name}");
        if (node is IMemorableNode memorableNode)
        {
            AddMemory(memorableNode.GetNodeLocationMemory());
        }
    }

    private bool AddMemory(MemoryEntry memoryEntry)
    {
        if (MemoryEntries.Contains(memoryEntry)) return false; // Memory entry already exists

        MemoryEntries.Add(memoryEntry);
        EmitSignal(SignalName.OnMemoryEntryAdded, memoryEntry);

        return true;
    }

    private void LogCurrentMemoryEntries()
    {
        // JSON serialization for better readability
        string memoryEntriesString = Json.Stringify(MemoryEntries.Select(entry => entry.ToString()).ToArray());
        GD.Print("Current Memory Entries: "+ memoryEntriesString);
    }

}
