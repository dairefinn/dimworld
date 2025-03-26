namespace Dimworld;

using Godot;


public partial class Torch : InventoryItem
{

    private PointLight2D lightSource = new()
    {
        Name = "Torch Light",
        Texture = GD.Load<Texture2D>("res://LightTexture.tres")
    };


    public override bool OnEquip(EquipmentHandler handler)
    {
        bool wasSuccessful = base.OnEquip(handler);
        if (!wasSuccessful) return false;

        // We override this so we can add the light source to the equipment handler of the character using the torch
        if (!handler.GetChildren().Contains(lightSource))
        {
            handler.AddChild(lightSource);
        }

        return true;
    }

    public override bool OnUnequip(EquipmentHandler handler)
    {
        bool wasSuccessful = base.OnUnequip(handler);
        if (!wasSuccessful) return false;

        // We override this so we can remove the light source from the equipment handler of the character using the torch
        if (handler.GetChildren().Contains(lightSource))
        {
            handler.RemoveChild(lightSource);
        }

        return true;
    }

    // TODO: Might be a cleaner way to do this. Maybe providing a list of options which each have a label, action and condition for displaying.
    public override InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler, bool itemIsInParentInventory)
    {
        InventoryContextMenuUI.ContextMenuOption[] options = [];

        if (itemIsInParentInventory)
        {
            if (IsEquipped)
            {
                options = [..options, new InventoryContextMenuUI.ContextMenuOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI, equipmentHandler))];
            }
            else
            {
                options = [..options, new InventoryContextMenuUI.ContextMenuOption("Equip", () => ContextOptionEquipTorch(contextMenuUI, equipmentHandler))];
            }
        }

        return options;
    }

    private void ContextOptionEquipTorch(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Equip");
        equipmentHandler.Equip(this);
        contextMenuUI.AddOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI, equipmentHandler));
    }

    private void ContextOptionUnequipTorch(InventoryContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Unequip");
        equipmentHandler.Unequip(this);
        contextMenuUI.AddOption("Equip", () => ContextOptionEquipTorch(contextMenuUI, equipmentHandler));
    }

}
