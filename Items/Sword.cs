namespace Dimworld;

using Dimworld.Effects;
using Godot;
using Godot.Collections;

public partial class Sword : InventoryItem, ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        GD.Print("Sword used from hotbar");

        float radius = 20.0f;
        float damage = -20f;
        float force = 1000.0f;
        Node parent = equipmentHandler.GetParent();
        
        if (parent is not CharacterController characterController) return true;
        CollisionShape2D hitbox = characterController.GetChildOrNull<CollisionShape2D>(0);
        if (hitbox == null) return true;
        CapsuleShape2D capsuleShape = hitbox.Shape as CapsuleShape2D;
        if (capsuleShape == null) return true;

        // Effects will be placed at <radius> units away in the direction of the cursor
        Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * radius);

        RectangleShape2D effectArea = new()
        {
            Size = new Vector2(radius, radius * 2),
        };
        Array<Node> nodeBlacklist = [parent];

        Effect damageEffect = new AddHealthEffect(effectArea, [1, 2], damage).SetDuration(0f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition);
        Effect knockbackEffect = new PushPullEffect(effectArea, [1, 2], force).SetDuration(0f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition);

        equipmentHandler.AddChild(damageEffect);
        equipmentHandler.AddChild(knockbackEffect);

        damageEffect.GlobalRotation = direction.Angle();
        knockbackEffect.GlobalRotation = direction.Angle();

        return true;
    }

}
