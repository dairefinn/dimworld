namespace Dimworld.Items.Weapons;

using Dimworld.Dialogue;
using Dimworld.Effects;
using Godot;
using Godot.Collections;

public partial class Revolver : InventoryItem, ICanBeUsedFromHotbar
{

    [Export] public float BulletRadius = 2.0f;
    [Export] public float BulletLength = 10.0f;
    [Export] public float BulletDamage = -20f;
    [Export] public float BulletSpeed = 200f;
    [Export] public int BulletCount { get; set; } = 6;


    private int _bulletsRemaining;


    public Revolver()
    {
        _bulletsRemaining = BulletCount;
    }


    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
    
        Node parent = equipmentHandler.GetParent();

        if (_bulletsRemaining <= 0)
        {
            if (parent is ICanSpeak canSpeak)
            {
                canSpeak.SpeechBubble.Say("Out of ammo!");
            }
            return false;
        }

        if (parent is not CharacterController characterController) return true;
        CollisionShape2D hitbox = characterController.GetChildOrNull<CollisionShape2D>(0);
        if (hitbox == null) return true;
        CapsuleShape2D capsuleShape = hitbox.Shape as CapsuleShape2D;
        if (capsuleShape == null) return true;

        // Effects will be placed at <radius> units away in the direction of the cursor
        Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * BulletRadius);

        CircleShape2D effectArea = new()
        {
            Radius = BulletRadius
        };
        Array<Node> nodeBlacklist = [parent];

        Effect damageEffect = new AddHealthEffect(effectArea, [1, 2], BulletDamage).SetDuration(10f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition).SetVelocity(direction * BulletSpeed);

        Globals.Instance.LevelHandler.CurrentLevel.AddChild(damageEffect);

        // TODO: Simple way of rendering the bullets visually using primitive meshes. Replace with an actual sprite.
        MeshInstance2D mesh = new()
        {
            Mesh = new CapsuleMesh()
            {
                Radius = BulletRadius,
                Height = BulletLength,
            }
        };
        mesh.Rotate(Mathf.DegToRad(90f));
        damageEffect.AddChild(mesh);

        damageEffect.GlobalRotation = direction.Angle();

        _bulletsRemaining--;

        return true;
    }

}
