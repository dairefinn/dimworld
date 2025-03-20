namespace Dimworld;

using Godot;

[GlobalClass]
public partial class InventoryItem : Resource
{

    [Export] public string id = "";
    [Export] public string ItemName = "Item";
    [Export] public string ItemDescription = "Item description";
    [Export] public Texture2D Icon = null;
    [Export] public int MaxStackSize = 1;
    
    public bool IsStackable => MaxStackSize > 1;

}
