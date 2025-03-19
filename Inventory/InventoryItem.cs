namespace Dimworld;

using Godot;

public partial class InventoryItem : Resource
{


    [Export] public string ItemName = "Item";
    [Export] public Texture Icon = null;
    [Export] public int MaxStackSize = 1;
    [Export] public bool IsStackable = false;

}
