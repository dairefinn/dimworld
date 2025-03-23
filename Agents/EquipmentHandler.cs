namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class EquipmentHandler : Node2D
{

    [Export] public Array<EquipmentSlot> Equipment { get; set; } = [];


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
        foreach (Node node in Equipment)
        {
            if (node is EquipmentSlot slot)
            {
                if (slot.Item == item)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public partial class EquipmentSlot : Node
    {
        public ICanBeEquipped Item { get; set; }
        public Node Node { get; set; }
    }

}
