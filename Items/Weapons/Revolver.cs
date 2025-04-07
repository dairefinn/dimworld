namespace Dimworld.Items.Weapons;

using Dimworld.Effects;
using Godot;
using Godot.Collections;

public partial class Revolver : InventoryItem, ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        float radius = 2.0f;
        float damage = -20f;
        float speed = 200f;
        Node parent = equipmentHandler.GetParent();

        if (parent is not CharacterController characterController) return true;
        CollisionShape2D hitbox = characterController.GetChildOrNull<CollisionShape2D>(0);
        if (hitbox == null) return true;
        CapsuleShape2D capsuleShape = hitbox.Shape as CapsuleShape2D;
        if (capsuleShape == null) return true;

        // Effects will be placed at <radius> units away in the direction of the cursor
        Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * radius);

        CircleShape2D effectArea = new()
        {
            Radius = radius
        };
        Array<Node> nodeBlacklist = [parent];

        Effect damageEffect = new AddHealthEffect(effectArea, [1, 2], damage).SetDuration(10f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition).SetVelocity(direction * speed);

        equipmentHandler.GetTree().Root.AddChild(damageEffect);

        // TODO: Simple way of rendering the bullets visually using primitive meshes. Replace with an actual sprite.
        MeshInstance2D mesh = new()
        {
            Mesh = new CapsuleMesh()
            {
                Radius = radius,
                Height = radius * 5f,
            }
        };
        mesh.Rotate(Mathf.DegToRad(90f));
        damageEffect.AddChild(mesh);

        damageEffect.GlobalRotation = direction.Angle();

        return true;
    }

}
