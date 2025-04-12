namespace Dimworld.Core.Effects;

using Dimworld.Core.Modifiers;
using Godot;


public abstract partial class ModifierEffect : Effect
{

    public ModifierEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not IAffectedByModifiers nodeHasModifiers) return; // Node must have Modifiers

        base.OnBodyEntered(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not IAffectedByModifiers nodeHasModifiers) return; // Node must have Modifiers

        base.OnBodyExited(body);
    }

    public override void TriggerEffectOnNode(Node node, double delta)
    {
        if (node is not IAffectedByModifiers nodeWithModifiers) return; // Node must have AgentStats
        TriggerEffectOnNode(node, nodeWithModifiers, delta);
    }

    public virtual void TriggerEffectOnNode(Node node, IAffectedByModifiers nodeWithModifiers, double delta)
    {
        // This method should be overridden in derived classes to apply specific effects to the node.
    }


}
