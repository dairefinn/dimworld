namespace Dimworld.Core.Items;

using Godot;


/// <summary>
/// Represents an item in the inventory.
/// This class is used to store any required data about the item.
/// </summary>
[GlobalClass]
public partial class InventoryItem : Resource
{

    [Export] public string Id { get; set; } = "";
    [Export] public string ItemName { get; set; } = "Item";
    [Export] public string ItemDescription { get; set; } = "Item description";
    [Export] public Texture2D Icon { get; set; } = null;
    [Export] public int MaxStackSize { get; set; } = 1;
    
    public bool IsStackable => MaxStackSize > 1;
    public virtual bool IsEquipped { get; set; }

}
