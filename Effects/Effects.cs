namespace Dimworld;

using Godot;


public class Effects
{

    public static readonly PackedScene DAMAGE_INSTANT = GD.Load<PackedScene>("res://Effects/DamageInstant/DamageInstant.tscn");
    public static readonly PackedScene DAMAGE_OVER_TIME = GD.Load<PackedScene>("res://Effects/DamageOverTime/DamageOverTime.tscn");
    public static readonly PackedScene PUSH_PULL = GD.Load<PackedScene>("res://Effects/PushPull/PushPull.tscn");
    public static readonly PackedScene APPLY_MODIFIER = GD.Load<PackedScene>("res://Effects/ApplyModifier/ApplyModifier.tscn");

}