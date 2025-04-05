namespace Dimworld;


public interface IHasContextMenu
{

    public InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, bool itemIsInParentInventory);

}
