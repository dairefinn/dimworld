namespace Dimworld.Items.Weapons;

using Dimworld.Core;
using Dimworld.Core.Effects;
using Dimworld.Core.Items;
using Dimworld.Effects;
using Godot;
using Godot.Collections;


public partial class Sword : InventoryItem, ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        float radius = 20.0f;
        float damage = -20f;
        float force = 1000.0f;
        Node parent = equipmentHandler.GetParent();

        // Effects will be placed at <radius> units away in the direction of the cursor
        Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * radius);

        RectangleShape2D effectArea = new()
        {
            Size = new Vector2(radius, radius * 2),
        };
        Array<Node> nodeBlacklist = [parent];

        Effect damageEffect = new AddHealthEffect(effectArea, [1, 2], damage).SetDuration(0f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition);
        Effect knockbackEffect = new PushPullEffect(effectArea, [1, 2], force).SetDirection(PushPullEffect.Direction.PUSH_STRAIGHT).SetDuration(0f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition);

        equipmentHandler.AddChild(damageEffect);
        equipmentHandler.AddChild(knockbackEffect);
        
        // TODO: Simple way of rendering the sword slash visually using primitive meshes. Replace with an actual sprite.
        MeshInstance2D mesh = new()
        {
            Mesh = new BoxMesh()
            {
                Size = new Vector3(radius, radius * 2f, 0.1f)
            }
        };
        damageEffect.AddChild(mesh);

        damageEffect.GlobalRotation = direction.Angle();

        damageEffect.GlobalRotation = direction.Angle();
        knockbackEffect.GlobalRotation = direction.Angle();

        return true;
    }

}
