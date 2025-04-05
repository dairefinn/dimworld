namespace Dimworld.Items;

using Godot;


public partial class Torch : InventoryItem, IHasContextMenu, ICanBeEquipped, ICanBeUsedFromHotbar
{

    private PointLight2D lightSource = new()
    {
        Name = "Torch Light",
        Texture = GD.Load<Texture2D>("res://LightTexture.tres"),
        ZIndex = -1
    };


    public bool OnEquip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return false;

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
        EmitSignal(SignalName.ItemEquipped, IsEquipped);

        return true;
    }

    public bool OnUnequip(EquipmentHandler handler)
    {
        if (!CanBeEquipped) return false;
        
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
        EmitSignal(SignalName.ItemEquipped, IsEquipped);

        return true;
    }

    // TODO: Might be a cleaner way to do this. Maybe providing a list of options which each have a label, action and condition for displaying.
    public InventoryContextMenuUI.ContextMenuOption[] GetContextMenuOptions(InventoryContextMenuUI contextMenuUI, bool itemIsInParentInventory)
    {
        InventoryContextMenuUI.ContextMenuOption[] options = [];

        if (itemIsInParentInventory)
        {
            if (IsEquipped)
            {
                options = [..options, new InventoryContextMenuUI.ContextMenuOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI))];
            }
            else
            {
                options = [..options, new InventoryContextMenuUI.ContextMenuOption("Equip", () => ContextOptionEquipTorch(contextMenuUI))];
            }
        }

        return options;
    }

    private void ContextOptionEquipTorch(InventoryContextMenuUI contextMenuUI)
    {
        contextMenuUI.RemoveOption("Equip");
        Globals.Instance.Player.EquipmentHandler.Equip(this);
        contextMenuUI.AddOption("Unequip", () => ContextOptionUnequipTorch(contextMenuUI));
    }

    private void ContextOptionUnequipTorch(InventoryContextMenuUI contextMenuUI)
    {
        contextMenuUI.RemoveOption("Unequip");
        Globals.Instance.Player.EquipmentHandler.Unequip(this);
        contextMenuUI.AddOption("Equip", () => ContextOptionEquipTorch(contextMenuUI));
    }

    public bool UseFromHotbar()
    {
        if (IsEquipped)
        {
            Globals.Instance.Player.EquipmentHandler.Unequip(this);
        }
        else
        {
            Globals.Instance.Player.EquipmentHandler.Equip(this);
        }

        return true;
    }

}
