namespace Dimworld.Effects;

using Godot;


/// <summary>
/// This effect will add health to any node that enters the area. A negative value will remove health.
/// </summary>
public partial class AddHealthEffect : AgentStatsEffect
{

    private float _amount = 0f;


    public AddHealthEffect(Shape2D hitboxShape, int[] collisionlayers, float amount) : base(hitboxShape, collisionlayers)
    {
        _amount = amount;
    }


    public override void TriggerEffectOnNode(Node node, IHasAgentStats nodeWithStats, double delta)
    {
        if (_amount > 0)
        {
            nodeWithStats.Stats.Heal(_amount); // Add health
        }
        else
        {
            nodeWithStats.Stats.TakeDamage(-_amount); // Remove health
        }
    }

}
