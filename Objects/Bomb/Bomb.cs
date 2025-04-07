namespace Dimworld.Objects;

using Dimworld.Effects;
using Dimworld.Modifiers;
using Godot;


public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{

    public void InteractWith()
    {
        // TriggerExplosion();
        // TriggerFire();
        // TriggerKnockback();
        // TriggerPull();
        // TriggerKnockbackExplosion();
        TriggerApplyModifier();
    }


    private void TriggerExplosion()
    {
        RectangleShape2D effectShape = new()
        {
            Size = new Vector2(200, 100)
        };

        Effect damageEffect = new AddHealthEffect(effectShape, [1, 2], -80f)
            .SetDuration(0.1f);
        AddChild(damageEffect);
    }

    private void TriggerFire()
    {
        CircleShape2D effectShape = new()
        {
            Radius = 100
        };

        Effect damageOverTimeEffect = new AddHealthEffect(effectShape, [1, 2], -10f)
            .SetDuration(30f)
            .SetInterval(2f)
            .SetSticky(true)
            ;
        AddChild(damageOverTimeEffect);
    }

    private void TriggerKnockback()
    {
        CapsuleShape2D effectShape = new()
        {
            Height = 200,
            Radius = 100
        };

        Effect knockbackEffect = new PushPullEffect(effectShape, [1, 2], 1000f)
            .SetDirection(PushPullEffect.Direction.PUSH)
            .SetDuration(0.1f)
            ;

        AddChild(knockbackEffect);
    }

    private void TriggerPull()
    {
        CircleShape2D effectShape = new()
        {
            Radius = 100
        };

        Effect pullEffect = new PushPullEffect(effectShape, [1, 2], 1000f)
            .SetDirection(PushPullEffect.Direction.PULL)
            .SetDuration(0.1f)
            ;

        AddChild(pullEffect);
    }

    private void TriggerKnockbackExplosion()
    {
        float duration = 0.1f;

        CircleShape2D effectShape = new()
        {
            Radius = 100
        };

        Effect knockbackEffect = new PushPullEffect(effectShape, [1, 2], 3000f)
            .SetDirection(PushPullEffect.Direction.PUSH)
            .SetDuration(duration)
            ;
        Effect damageEffect = new AddHealthEffect(effectShape, [1, 2], -50f)
            .SetDuration(duration);

        AddChild(knockbackEffect);
        AddChild(damageEffect);
    }

    private void TriggerApplyModifier()
    {
        // Modifier modifier = new VelocityMultiplyModifier("bomb_speed_boost", 2f).SetDuration(5);
        Modifier modifier = new VelocityMultiplyModifier("bomb_slow_down", 0.5f).SetDuration(5);
        // Modifier modifier = new HealthMultiplyModifier("bomb_health_boost", 2f).SetHealOnAdd(true).SetDuration(5);

        CircleShape2D effectShape = new()
        {
            Radius = 100
        };

        Effect applyModifierEffect = new AddModifierEffect(effectShape, [1, 2], modifier)
            .SetDuration(0.1f)
            ;

        AddChild(applyModifierEffect);
    }

}
