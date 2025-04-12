namespace Dimworld.Core.Items;


/// <summary>
/// This interface is implemented by any item which has a context menu.
/// </summary>
public interface IHasContextMenu
{

    public ContextMenuOption[] GetContextMenuOptions(IContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler, bool itemIsInParentInventory);

}
