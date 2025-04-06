namespace Dimworld.Effects;

using Dimworld.Modifiers;
using Godot;


/// <summary>
/// This effect will add a modifier to all nodes that enter the area.
/// </summary>
public partial class AddModifierEffect : ModifierEffect
{

    private Modifier _modifier = null;

    public AddModifierEffect(Shape2D hitboxShape, int[] collisionlayers, Modifier modifier) : base(hitboxShape, collisionlayers)
    {
        _modifier = modifier;
    }


    public override void TriggerEffect(double delta)
    {
        foreach(Node node in DetectedNodes)
        {
            if (node is not IAffectedByModifiers nodeHasModifiers) return; // Node must have Modifiers

            nodeHasModifiers.ModifierHandler?.Add(_modifier);
        }
    }

}
