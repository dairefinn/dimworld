namespace Dimworld.Core.Items;


/// <summary>
/// This interface is implemented by any item which can be used from the hotbar.
/// </summary>
public interface ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler);

}