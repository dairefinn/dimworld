namespace Dimworld.Core.Items;


/// <summary>
/// This interface is implemented by any item which can be equipped.
/// </summary>
public interface ICanBeEquipped
{

    public bool OnEquip(EquipmentHandler handler);

    public bool OnUnequip(EquipmentHandler handler);

}
