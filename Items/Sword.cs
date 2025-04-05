namespace Dimworld;

using Godot;


public partial class Sword : InventoryItem, ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        GD.Print("TODO: Implement weapon usage from hotbar");
        return true;
    }

}
