namespace Dimworld.Items;


public interface ICanBeEquipped
{

    public bool OnEquip(EquipmentHandler handler);

    public bool OnUnequip(EquipmentHandler handler);

}
