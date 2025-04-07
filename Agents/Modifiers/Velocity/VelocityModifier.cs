namespace Dimworld.Modifiers;

using System.Reflection.Emit;
using Godot;


public abstract partial class VelocityModifier : Modifier
{

    public VelocityModifier(string key) : base(key)
    {
    }

    /// <summary>
    /// Applies the modifier to the given velocity.
    /// </summary>
    /// <param name="velocity">The velocity to modify.</param>
    /// <returns>The modified velocity.</returns>
    public virtual Vector2 ApplyTo(Vector2 velocity)
    {
        return velocity;
    }

}