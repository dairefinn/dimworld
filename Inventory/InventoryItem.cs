namespace Dimworld;

using Godot;

[GlobalClass]
public partial class InventoryItem : Resource, ICanBeEquipped // TODO: Every item has ICanBeEquipped for now but it might be better to add it per item type so we can determine if it's equippable or not
{

    [Export] public string Id { get; set; } = "";
    [Export] public string ItemName { get; set; } = "Item";
    [Export] public string ItemDescription { get; set; } = "Item description";
    [Export] public Texture2D Icon { get; set; } = null;
    [Export] public int MaxStackSize { get; set; } = 1;

    [Export] public bool CanBeEquipped { get; set; }
    
    public bool IsStackable => MaxStackSize > 1;

    // EQUIPMENT HANDLING

    public virtual void OnEquip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return;
        return;
    }

    public virtual void OnUnequip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return;
        return;
    }

}
