namespace Dimworld;

using Dimworld.Modifiers;
using Godot;


public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{

    public static readonly PackedScene EFFECT_EXPLOSION = GD.Load<PackedScene>("res://Effects/DamageInstant/DamageInstant.tscn");
    public static readonly PackedScene EFFECT_DAMAGE_OVER_TIME = GD.Load<PackedScene>("res://Effects/DamageOverTime/DamageOverTime.tscn");
    public static readonly PackedScene EFFECT_PUSH_PULL = GD.Load<PackedScene>("res://Effects/PushPull/PushPull.tscn");
    public static readonly PackedScene EFFECT_APPLY_MODIFIER = GD.Load<PackedScene>("res://Effects/ApplyModifier/ApplyModifier.tscn");


    public void InteractWith()
    {
        // TriggerExplosion();
        // TriggerFire();
        // TriggerKnockback();
        // TriggerPull();
        TriggerApplyModifier();
    }

    private void TriggerExplosion()
    {
        DamageInstant explosion = EFFECT_EXPLOSION.Instantiate<DamageInstant>();
        explosion.Damage = 100;
        explosion.Radius = 200;
        explosion.Duration = 1;
        AddChild(explosion);
    }

    private void TriggerFire()
    {
        DamageOverTime fire = EFFECT_DAMAGE_OVER_TIME.Instantiate<DamageOverTime>();
        fire.Damage = 10;
        fire.Radius = 100;
        fire.Duration = 60;
        fire.DamageInterval = 1f;
        AddChild(fire);
    }

    private void TriggerKnockback()
    {
        PushPull knockback = EFFECT_PUSH_PULL.Instantiate<PushPull>();
        knockback.Radius = 100;
        knockback.Force = 1000;
        knockback.Direction = PushPull.DirectionType.PUSH;
        AddChild(knockback);
    }

    private void TriggerPull()
    {
        PushPull pull = EFFECT_PUSH_PULL.Instantiate<PushPull>();
        pull.Radius = 100;
        pull.Force = 1000;
        pull.Direction = PushPull.DirectionType.PULL;
        AddChild(pull);
    }

    private void TriggerKnockbackExplosion()
    {
        float duration = 0.1f;

        DamageInstant explosion = EFFECT_EXPLOSION.Instantiate<DamageInstant>();
        explosion.Damage = 10;
        explosion.Radius = 200;
        explosion.Duration = duration;

        PushPull knockback = EFFECT_PUSH_PULL.Instantiate<PushPull>();
        knockback.Radius = 100;
        knockback.Force = 3000;
        knockback.Direction = PushPull.DirectionType.PUSH;

        AddChild(explosion);
        AddChild(knockback);
    }

    private void TriggerApplyModifier()
    {
        ApplyModifier applyModifier = EFFECT_APPLY_MODIFIER.Instantiate<ApplyModifier>();
        // applyModifier.Modifier = new VelocityMultiplyModifier("bomb_speed_boost", 2f).SetDuration(5);
        // applyModifier.Modifier = new VelocityMultiplyModifier("bomb_slow_down", 0.5f).SetDuration(5);
        applyModifier.Modifier = new HealthMultiplyModifier("bomb_health_boost", 2f).SetHealOnAdd(true).SetDuration(5);
        applyModifier.Radius = 100;
        applyModifier.Duration = 5;
        AddChild(applyModifier);
    }

}
