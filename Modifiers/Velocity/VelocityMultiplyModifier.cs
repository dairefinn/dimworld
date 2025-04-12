namespace Dimworld.Modifiers;

using Godot;


public partial class VelocityMultiplyModifier : VelocityModifier
{

    public float Multiplier { get; set; } = 1f;


    public VelocityMultiplyModifier(string key, float multiplier) : base(key)
    {
        Multiplier = multiplier;
    }


    public override Vector2 ApplyTo(Vector2 velocity)
    {
        return velocity * Multiplier;
    }

}
