namespace Dimworld.Items;


public interface IHasContextMenu
{

    public InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler, bool itemIsInParentInventory);

}
