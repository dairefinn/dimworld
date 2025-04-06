namespace Dimworld.Effects;

using Godot;


public abstract partial class MovementEffect : Effect
{

    public MovementEffect(Shape2D hitboxShape, int[] collisionlayers) : base(hitboxShape, collisionlayers)
    {
    }


    public override void OnBodyEntered(Node body)
    {
        if (body is not ICanBeMoved nodeCanBeMoved) return; // Node must have ICanBeMoved

        AddDetectedNode(body);
    }

    public override void OnBodyExited(Node body)
    {
        if (body is not ICanBeMoved nodeCanBeMoved) return; // Node must have ICanBeMoved

        RemoveDetectedNode(body);
    }

}
