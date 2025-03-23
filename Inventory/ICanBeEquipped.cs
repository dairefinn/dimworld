namespace Dimworld;

using Godot;


public interface ICanBeEquipped
{

    public bool CanBeEquipped { get; set; }


    public void OnEquip(EquipmentHandler handler);

    public void OnUnequip(EquipmentHandler handler);

}
