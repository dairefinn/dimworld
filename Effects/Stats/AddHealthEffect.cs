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


    public override void TriggerEffect(double delta)
    {
        foreach(Node node in DetectedNodes)
        {
            if (node is not IHasAgentStats nodeWithStats) return; // Node must have AgentStats
            if (nodeWithStats.Stats == null) return; // Node must have AgentStats

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

}
