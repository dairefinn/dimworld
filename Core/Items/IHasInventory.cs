namespace Dimworld.Core.Items;

using Godot;


/// <summary>
/// Implemented by any entity which has an inventory attached to it.
/// </summary>
public interface IHasInventory
{

    [Export] public Inventory Inventory { get; set; }

}