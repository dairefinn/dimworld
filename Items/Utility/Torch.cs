namespace Dimworld.Items.Utility;

using Dimworld.Core.Items;
using Godot;


public partial class Torch : InventoryItem, IHasContextMenu, ICanBeEquipped, ICanBeUsedFromHotbar
{

    private PointLight2D lightSource = new()
    {
        Name = "Torch Light",
        Texture = GD.Load<Texture2D>("res://Assets/Textures/LightTexture.tres"),
        ZIndex = -1
    };


    public bool OnEquip(EquipmentHandler handler)
    {
        // TODO: Equipment should go in slots
        // TODO: Move this to the equipment handler
        if (!handler.Equipment.Contains(this))
        {
            handler.Equipment.Add(this);
        }

        if (!handler.GetChildren().Contains(lightSource))
        {
            handler.AddChild(lightSource);
        }

        IsEquipped = true;

        return true;
    }

    public bool OnUnequip(EquipmentHandler handler)
    {        
        // TODO: Equipment should go in slots
        // TODO: Move this to the equipment handler
        if (handler.Equipment.Contains(this))
        {
            handler.Equipment.Remove(this);
        }

        // We override this so we can remove the light source from the equipment handler of the character using the torch
        if (handler.GetChildren().Contains(lightSource))
        {
            handler.RemoveChild(lightSource);
        }

        IsEquipped = false;

        return true;
    }

    // TODO: Might be a cleaner way to do this. Maybe providing a list of options which each have a label, action and condition for displaying.
    public ContextMenuOption[] GetContextMenuOptions(IContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler, bool itemIsInParentInventory)
    {
        ContextMenuOption[] options = [];

        if (itemIsInParentInventory)
        {
            if (IsEquipped)
            {
                options = [..options, new ContextMenuOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI, equipmentHandler))];
            }
            else
            {
                options = [..options, new ContextMenuOption("Equip", () => ContextOptionEquipTorch(contextMenuUI, equipmentHandler))];
            }
        }

        return options;
    }

    private void ContextOptionEquipTorch(IContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Equip");
        equipmentHandler.Equip(this);
        contextMenuUI.AddOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI, equipmentHandler));
    }

    private void ContextOptionUnequipTorch(IContextMenuUI contextMenuUI, EquipmentHandler equipmentHandler)
    {
        contextMenuUI.RemoveOption("Unequip");
        equipmentHandler.Unequip(this);
        contextMenuUI.AddOption("Equip", () => ContextOptionEquipTorch(contextMenuUI, equipmentHandler));
    }

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        if (IsEquipped)
        {
            equipmentHandler.Unequip(this);
        }
        else
        {
            equipmentHandler.Equip(this);
        }

        return true;
    }

}
