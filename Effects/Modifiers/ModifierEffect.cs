namespace Dimworld.Effects;

using Dimworld.Modifiers;
using Godot;


public abstract partial class ModifierEffect : Effect
{

    public ModifierEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not IAffectedByModifiers nodeHasModifiers) return; // Node must have Modifiers

        AddDetectedNode(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not IAffectedByModifiers nodeHasModifiers) return; // Node must have Modifiers

        RemoveDetectedNode(body);
    }

}
