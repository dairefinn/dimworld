namespace Dimworld.Entities.Objects;

using Dimworld.Core.Effects;
using Dimworld.Core.Interaction;
using Dimworld.Core.Modifiers;
using Dimworld.Effects;
using Godot;


/// <summary>
/// A bomb that can be interacted with which will trigger an effect when it explodes.
/// This is used for testing the effects system.
/// </summary>
public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{
    
    /// <summary>
    /// Creates an effect mimicking an explosion around the bomb.
    /// This does instant damage to all entities in the area and then disappears.
    /// </summary>
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

    /// <summary>
    /// Creates an effect mimicking a fire around the bomb.
    /// This does damage over time to all entities in the area and lasts for a while.
    /// The damage will stick to any entities that enter the area.
    /// </summary>
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

    /// <summary>
    /// Creates a knockback effect around the bomb.
    /// </summary>
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

    /// <summary>
    /// Creates a pull effect around the bomb.
    /// </summary>
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

    /// <summary>
    /// Creates a version of the explosion effect that also knocks back any entities in the area.
    /// </summary>
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

    /// <summary>
    /// Creates an area around the bomb which will apply a given effect to any entities that enter it.
    /// </summary>
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


    // INTERFACES

    public void InteractWith()
    {
        TriggerExplosion();
        // TriggerFire();
        // TriggerKnockback();
        // TriggerPull();
        // TriggerKnockbackExplosion();
        // TriggerApplyModifier();
    }

}
