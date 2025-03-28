namespace Dimworld;

using Godot;


public partial class Bomb : StaticBody2D, ICanBeInteractedWith
{

    public static readonly PackedScene EXPLOSION_EFFECT = GD.Load<PackedScene>("res://Effects/Explosion/Explosion.tscn");

    private const int BOMB_DAMAGE = 100;
    private const float BOMB_RADIUS = 200f;
    private const float BOMB_DURATION = 1f;

    public void InteractWith()
    {
        TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        Explosion explosion = EXPLOSION_EFFECT.Instantiate<Explosion>();
        explosion.Damage = BOMB_DAMAGE;
        explosion.Radius = BOMB_RADIUS;
        explosion.Duration = BOMB_DURATION;
        AddChild(explosion);
    }

}
