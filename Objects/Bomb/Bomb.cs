namespace Dimworld;

using Godot;


public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{

    public static readonly PackedScene EXPLOSION_EFFECT = GD.Load<PackedScene>("res://Effects/Explosion/Explosion.tscn");
    public static readonly PackedScene FIRE_EFFECT = GD.Load<PackedScene>("res://Effects/Fire/Fire.tscn");


    public void InteractWith()
    {
        // TriggerExplosion();
        TriggerFire();
    }

    private void TriggerExplosion()
    {
        Explosion explosion = EXPLOSION_EFFECT.Instantiate<Explosion>();
        explosion.Damage = 100;
        explosion.Radius = 200;
        explosion.Duration = 1;
        AddChild(explosion);
    }

    private void TriggerFire()
    {
        Fire fire = FIRE_EFFECT.Instantiate<Fire>();
        fire.Damage = 10;
        fire.Radius = 100;
        fire.Duration = 60;
        fire.DamageInterval = 1f;
        AddChild(fire);
    }

}
