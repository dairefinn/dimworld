namespace Dimworld.Core.Characters.Memory;

using Dimworld.Core.Characters.Memory.MemoryEntries;



/// <summary>
/// An interface for nodes that can be remembered by an agent.
/// </summary>
public interface IMemorableNode
{

    /// <summary>
    /// Generates a memory entry for the node which contains the node's location and a reference to the node itself.
    /// </summary>
    /// <returns></returns>
    public NodeLocation GetNodeLocationMemory();

}
