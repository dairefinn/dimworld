namespace Dimworld;

using Godot;

[Tool]
[GlobalClass]
public partial class InventoryItem : Resource, ICanBeEquipped, IHasContextMenu // TODO: Every item has ICanBeEquipped for now but it might be better to add it per item type so we can determine if it's equippable or not
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


    // EQUIPMENT HANDLING

    public virtual bool OnEquip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return false;

        // TODO: Equipment should go in slots
        if (!handler.Equipment.Contains(this))
        {
            handler.Equipment.Add(this);
        }

        IsEquipped = true;

        EmitSignal(SignalName.ItemEquipped, IsEquipped);

        return true;
    }

    public virtual bool OnUnequip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return false;
        
        // TODO: Equipment should go in slots
        if (handler.Equipment.Contains(this))
        {
            handler.Equipment.Remove(this);
        }

        IsEquipped = false;

        EmitSignal(SignalName.ItemEquipped, IsEquipped);

        return true;
    }


    // CONTEXT MENU

    public virtual InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler, bool itemIsInParentInventory)
    {
        return null;
    }

}
