namespace Dimworld.Core.Characters.Memory;

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

    /// <summary>
    /// The time in seconds until this memory entry expires.
    /// </summary>
    public float ExpirationTime { get; set; } = 300f; // 5 minutes by default


    /// <summary>
    /// Should be called when the memory entry is processed every frame.
    /// </summary>
    /// <param name="delta">The delta time since the last frame.</param>
    public virtual void OnProcess(double delta)
    {
        ExpirationTime -= (float)delta;
    }

    /// <summary>
    /// Checks if the memory entry has expired.
    /// </summary>
    /// <returns>True if the memory entry has expired, false otherwise.</returns>
    public virtual bool IsExpired()
    {
        return ExpirationTime <= 0f;
    }

    /// <summary>
    /// Checks if this memory entry is related to a specific node.
    /// </summary>
    /// <param name="node">The node to check against.</param>
    /// <returns>True if the memory entry is related to the node, false otherwise.</returns>
    public virtual bool IsRelatedToNode(Node node)
    {
        return false;
    }

    /// <summary>
    /// Checks if this memory entry is related to a specific node.
    /// </summary>
    /// <param name="memoryEntries">An array of memory entries to check against.</param>
    /// <returns>The first matching memory entry, or null if none is found.</returns>
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

    /// <summary>
    /// Checks if this memory entry is equal to another memory entry.
    /// </summary>
    /// <param name="other">The other memory entry to compare against.</param>
    /// <returns>True if the memory entries are equal, false otherwise.</returns>
    public virtual bool Equals(MemoryEntry other)
    {
        return base.Equals(other);
    }

}
