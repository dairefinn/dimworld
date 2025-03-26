namespace Dimworld;

using Godot;


public interface ICanBeEquipped
{

    public bool CanBeEquipped { get; set; }


    public bool OnEquip(EquipmentHandler handler);

    public bool OnUnequip(EquipmentHandler handler);

}
