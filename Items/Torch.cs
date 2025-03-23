namespace Dimworld;

using System.Linq;
using Godot;


public partial class Torch : InventoryItem
{

    private Texture2D lightTexture = GD.Load<Texture2D>("res://LightTexture.tres");

    private PointLight2D lightSource;

    public override void OnEquip(EquipmentHandler handler)
    {
        PointLight2D lightSource = new()
        {
            Texture = lightTexture
        };
        handler.AddChild(lightSource);
        handler.Equipment.Add(new EquipmentHandler.EquipmentSlot { Item = this, Node = lightSource });
    }

    public override void OnUnequip(EquipmentHandler handler)
    {
        EquipmentHandler.EquipmentSlot slot = handler.Equipment.Where(slot => slot.Item == this).First();
        if (slot == null) return;

        handler.RemoveChild(slot.Node);
        handler.Equipment.Remove(slot);
    }

}
