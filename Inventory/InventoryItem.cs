namespace Dimworld;

using Godot;


[GlobalClass]
public partial class InventoryItem : Resource
{

    [Signal] public delegate void ItemEquippedEventHandler(bool equipped);

    [Export] public string Id { get; set; } = "";
    [Export] public string ItemName { get; set; } = "Item";
    [Export] public string ItemDescription { get; set; } = "Item description";
    [Export] public Texture2D Icon { get; set; } = null;
    [Export] public int MaxStackSize { get; set; } = 1;

    [Export] public bool CanBeEquipped { get; set; }
    
    public bool IsStackable => MaxStackSize > 1;
    public virtual bool IsEquipped { get; set; }

}
