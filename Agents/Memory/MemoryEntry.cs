namespace Dimworld;

using Godot;
using Godot.Collections;


/// <summary>
/// An individual memory of an agent.
/// Some examples:
/// - The location of an object (e.g. Chest)
/// - The location of an agent (e.g. Player)
/// - The contents of a chest (e.g. "item-sword")
/// - A path they're supposed to patrol during the patrol action
/// </summary>
public partial class MemoryEntry : Resource
{

    public virtual bool IsRelatedToNode(Node node)
    {
        return false;
    }

    public virtual MemoryEntry GetMatchingEntryFrom(Array<MemoryEntry> memoryEntries)
    {
        foreach (MemoryEntry entry in memoryEntries)
        {
            if (entry == this)
            {
                return entry;
            }
        }

        return null;
    }

    public virtual bool Equals(MemoryEntry other)
    {
        return base.Equals(other);
    }

}
