namespace Dimworld.Effects;

using Godot;


/// <summary>
/// This effect will add health to any node that is in the area over time. A negative value will remove health.
/// </summary>
public partial class AddHealthOverTimeEffect : AgentStatsEffect
{

    private float _amount = 0f;


    public AddHealthOverTimeEffect(Shape2D hitboxShape, int[] collisionlayers, float amount) : base(hitboxShape, collisionlayers)
    {
        _amount = amount;
    }


    public override void OnInterval()
    {
        GD.Print($"AddHealthOverTimeEffect: OnInterval() called, amount: {_amount}");
        foreach (Node node in DetectedNodes)
        {
            if (node is not IHasAgentStats characterController) continue; // Node must have AgentStats
            if (characterController.Stats == null) continue; // Node must have AgentStats

            if (_amount > 0)
            {
                characterController.Stats.Heal(_amount); // Add health
            }
            else
            {
                characterController.Stats.TakeDamage(-_amount); // Remove health
            }
        }

        base.OnInterval();
    }

}
