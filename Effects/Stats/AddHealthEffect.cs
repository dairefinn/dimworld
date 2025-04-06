namespace Dimworld.Effects;

using Godot;


/// <summary>
/// This effect will add health to any node that enters the area. A negative value will remove health.
/// </summary>
public partial class AddHealthEffect : AgentStatsEffect
{

    private float _amount = 0f;

    public AddHealthEffect(Shape2D hitboxShape, uint collisionlayer, float amount) : base(hitboxShape, collisionlayer)
    {
        _amount = amount;
    }

    public override void AddDetectedNode(Node node)
    {
        base.AddDetectedNode(node);

        if (node is not IHasAgentStats characterController) return; // Node must have AgentStats
        if (characterController.Stats == null) return; // Node must have AgentStats

        if (_amount > 0)
        {
            characterController.Stats.Heal(_amount); // Add health
        }
        else
        {
            characterController.Stats.TakeDamage(-_amount); // Remove health
        }
    }

}
