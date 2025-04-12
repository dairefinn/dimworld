namespace Dimworld.Core.Items;

using Godot;
using Godot.Collections;


// TODO: Probably want to prevent items being equipped sometimes if a conflicting item is already equipped. For example, two helmets.
/// <summary>
/// The EquipmentHandler class is responsible for managing the equipment of an entity.
/// </summary>
public partial class EquipmentHandler : Node2D
{

    [Export] public Array<InventoryItem> Equipment { get; set; } = [];

    public Node2D Parent { get => _parent; }
    private Node2D _parent;


    public EquipmentHandler(Node2D parent)
    {
        Name = "EquipmentHandler";
        _parent = parent;
        _parent.AddChild(this);
    }


    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        GlobalPosition = (GetParent() as Node2D).GlobalPosition;
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
