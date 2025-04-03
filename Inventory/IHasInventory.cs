namespace Dimworld;

using Godot;


public interface IHasInventory
{

    [Export] public Inventory Inventory { get; set; }

    /// <summary>
    /// This is used to determine if the inventory is accessible to other agents.
    /// </summary>
    /// <returns>True if the inventory is accessible to other agents, false otherwise.</returns>
    [Export] public bool CanTakeFromInventory { get; set; }

}