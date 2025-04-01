namespace Dimworld.MemoryEntries;

using Godot;
using Godot.Collections;

public partial class NodeLocation : MemoryEntry
{

    public Node2D Node { get; set; } = null;
    public Vector2 Position { get; set; } = Vector2.Zero;

    public override string ToString()
    {
        if (Node != null)
        {
            return $"{Node.Name} {Node.GlobalPosition}";
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
        if (other is NodeLocation otherNodeLocation)
        {
            return Node == otherNodeLocation.Node && Position == otherNodeLocation.Position;
        }

        return false;
    }

}
