namespace Dimworld;

using Godot;
using Godot.Collections;

// TODO: Probably want to prevent items being equipped sometimes if a conflicting item is already equipped. For example, two helmets.
public partial class EquipmentHandler : Node2D
{

    [Export] public Array<InventoryItem> Equipment { get; set; } = [];


    public override void _Ready()
    {
        base._Ready();
    }

    public void Equip(ICanBeEquipped item)
    {
        if (item == null) return;
        if (IsEquipped(item))
        {
            Unequip(item);
            return;
        }
        else
        {
            item.OnEquip(this);
        }
    }

    public void Unequip(ICanBeEquipped item)
    {
        if (item == null) return;
        item.OnUnequip(this);
    }

    public bool IsEquipped(ICanBeEquipped item)
    {
        foreach (InventoryItem node in Equipment)
        {
            if (node is ICanBeEquipped equippedItem && equippedItem == item)
            {
                return true;
            }
        }

        return false;
    }

}
