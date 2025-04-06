namespace Dimworld;

using Dimworld.Effects;
using Dimworld.Modifiers;
using Godot;


public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{

    public void InteractWith()
    {
        TriggerExplosion();
        // TriggerFire();
        // TriggerKnockback();
        // TriggerPull();
        // TriggerApplyModifier();
    }

    private void TriggerExplosion()
    {
        RectangleShape2D effectShape = new()
        {
            Size = new Vector2(200, 100)
        };

        uint collisionLayers = 0b00000011; // Target layers 1 and 2
        AddHealthEffect effect = (AddHealthEffect) new AddHealthEffect(effectShape, collisionLayers, -100f)
            .SetDuration(60f);
        AddChild(effect);
    }

    // private void TriggerFire()
    // {
    //     DamageOverTime fire = Effect.DAMAGE_OVER_TIME.Instantiate<DamageOverTime>();
    //     fire.Damage = 10;
    //     fire.Radius = 100;
    //     fire.Duration = 60;
    //     fire.DamageInterval = 1f;
    //     AddChild(fire);
    // }

    // private void TriggerKnockback()
    // {
    //     PushPull knockback = Effect.PUSH_PULL.Instantiate<PushPull>();
    //     knockback.Radius = 100;
    //     knockback.Force = 1000;
    //     knockback.Direction = PushPull.DirectionType.PUSH;
    //     AddChild(knockback);
    // }

    // private void TriggerPull()
    // {
    //     PushPull pull = Effect.PUSH_PULL.Instantiate<PushPull>();
    //     pull.Radius = 100;
    //     pull.Force = 1000;
    //     pull.Direction = PushPull.DirectionType.PULL;
    //     AddChild(pull);
    // }

    // private void TriggerKnockbackExplosion()
    // {
    //     float duration = 0.1f;

    //     DamageInstant explosion = Effect.DAMAGE_INSTANT.Instantiate<DamageInstant>();
    //     explosion.Damage = 10;
    //     explosion.Radius = 200;
    //     explosion.Duration = duration;

    //     PushPull knockback = Effect.PUSH_PULL.Instantiate<PushPull>();
    //     knockback.Radius = 100;
    //     knockback.Force = 3000;
    //     knockback.Direction = PushPull.DirectionType.PUSH;

    //     AddChild(explosion);
    //     AddChild(knockback);
    // }

    // private void TriggerApplyModifier()
    // {
    //     ApplyModifier applyModifier = Effect.APPLY_MODIFIER.Instantiate<ApplyModifier>();
    //     // applyModifier.Modifier = new VelocityMultiplyModifier("bomb_speed_boost", 2f).SetDuration(5);
    //     // applyModifier.Modifier = new VelocityMultiplyModifier("bomb_slow_down", 0.5f).SetDuration(5);
    //     applyModifier.Modifier = new HealthMultiplyModifier("bomb_health_boost", 2f).SetHealOnAdd(true).SetDuration(5);
    //     applyModifier.Radius = 100;
    //     applyModifier.Duration = 5;
    //     AddChild(applyModifier);
    // }

}
