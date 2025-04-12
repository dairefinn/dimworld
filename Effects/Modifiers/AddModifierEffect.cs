namespace Dimworld.Effects;

using Dimworld.Core.Effects;
using Dimworld.Core.Modifiers;
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


    public override void TriggerEffectOnNode(Node node, IAffectedByModifiers nodeHasModifiers, double delta)
    {
        nodeHasModifiers.ModifierHandler?.Add(_modifier);
    }

}
