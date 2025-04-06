namespace Dimworld;

using Godot;


public partial class Sword : InventoryItem, ICanBeUsedFromHotbar
{

    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        GD.Print("Sword used from hotbar");
        // float radius = 25.0f;
        // float duration = 0.1f;
        // int damage = 10;
        // float force = 500.0f;

        // // TODO: Need to add a way to blacklist effects so they don't get added to the player when they attack
        // Node parent = equipmentHandler.GetParent();
        // if (parent is not CharacterController characterController) return true;
        // CollisionShape2D hitbox = characterController.GetChildOrNull<CollisionShape2D>(0);
        // if (hitbox == null) return true;
        // CapsuleShape2D capsuleShape = hitbox.Shape as CapsuleShape2D;
        // if (capsuleShape == null) return true;
        // float offset = (capsuleShape.Height / 2) + 5;
        
        // // Effects will be placed at <radius> units away in the direction of the cursor
        // Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        // Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * (radius + offset));

        // DamageInstant damageInstant = Effect.DAMAGE_INSTANT.Instantiate<DamageInstant>();
        // damageInstant.Duration = duration;
        // damageInstant.Radius = radius;
        // damageInstant.Damage = damage;

        // // PushPull knockback = Effects.PUSH_PULL.Instantiate<PushPull>();
        // // knockback.Duration = duration;
        // // knockback.Radius = radius;
        // // knockback.Force = force;
        // // knockback.Direction = PushPull.DirectionType.PUSH;

        // equipmentHandler.AddChild(damageInstant);
        // damageInstant.GlobalPosition = effectPosition;
        // // equipmentHandler.AddChild(knockback);
        // // knockback.GlobalPosition = effectPosition;

        // GD.Print($"Sword used from hotbar, effect position: {effectPosition}");

        return true;
    }

}
