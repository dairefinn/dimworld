namespace Dimworld;

using Godot;


public interface IHasInventory
{

    [Export] public Inventory Inventory { get; set; }

}