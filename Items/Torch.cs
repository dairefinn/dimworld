namespace Dimworld;

using Godot;


public partial class Torch : InventoryItem
{

    private PointLight2D lightSource = new()
    {
        Name = "Torch Light",
        Texture = GD.Load<Texture2D>("res://LightTexture.tres")
    };

    public override void OnEquip(EquipmentHandler handler)
    {
        handler.Equipment.Add(this);
        handler.AddChild(lightSource);
    }

    public override void OnUnequip(EquipmentHandler handler)
    {
        handler.Equipment.Remove(this);
        handler.RemoveChild(lightSource);
    }

    public override InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        InventoryContextMenuUI.ContextMenuOption[] options = [];

        if (lightSource.GetParent() == null)
        {
            options = [..options, new InventoryContextMenuUI.ContextMenuOption("Equip", () => EquipTorch(contextMenuUI, equipmentHandler))];
        }
        else
        {
            options = [..options, new InventoryContextMenuUI.ContextMenuOption("Unequip", () => UnequipTorch(contextMenuUI, equipmentHandler))];
        }

        return options;
    }

    private void EquipTorch(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Equip");
        equipmentHandler.Equip(this);
        contextMenuUI.AddOption("Unequip", () => UnequipTorch(contextMenuUI, equipmentHandler));

    }

    private void UnequipTorch(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Unequip");
        equipmentHandler.Unequip(this);
        contextMenuUI.AddOption("Equip", () => EquipTorch(contextMenuUI, equipmentHandler));
    }

}
