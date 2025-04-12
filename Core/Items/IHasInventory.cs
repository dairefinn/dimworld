namespace Dimworld.Core.Items;

using Godot;


/// <summary>
/// Implemented by any entity which has an inventory attached to it.
/// </summary>
public interface IHasInventory
{

    [Export] public Inventory Inventory { get; set; }

    // TODO: Instead of this being a hard yes/no, it might be better to make it a permissions system so we can control access on a per-character basis. This would allow for some interesting mechanics like a character being able to steal from another or one that searches you for contraband.
    /// <summary>
    /// This is used to determine if the inventory is accessible to other agents.
    /// </summary>
    /// <returns>True if the inventory is accessible to other agents, false otherwise.</returns>
    [Export] public bool CanTakeFromInventory { get; set; }

}