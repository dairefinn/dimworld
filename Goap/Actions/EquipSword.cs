namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class EquipSword : GoapAction
{

    private Chest detectedSword;

    // TODO: Get closest sword that can be reached instead of doing both checks separately
    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        Array<Chest> detectedSwords = agentBrain.DetectionHandler.GetDetectedInstancesOf<Chest>();
        if (detectedSwords.Count == 0) return false;

        Vector2 agentPosition = agentBrain.MovementController.GlobalPosition;
        detectedSword = detectedSwords[0];
        for(int i = 1; i < detectedSwords.Count; i++)
        {
            Chest sword = detectedSwords[i];
            if (agentPosition.DistanceTo(sword.GlobalPosition) < agentPosition.DistanceTo(detectedSword.GlobalPosition))
            {
                detectedSword = sword;
            }
        }

        return agentBrain.MovementController.CanReachPoint(detectedSword.GlobalPosition);
    }

    // {
    //     detectedSword = agentBrain.DetectedEntities.OfType<Sword>().FirstOrDefault();
    //     if (detectedSword != null)
    //     {
    //         // GD.Print("Can see sword");
    //         return true;
    //     }
    //     else
    //     {
    //         // GD.Print("Cannot see sword");
    //         return false;
    //     }
    // }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        if (detectedSword == null) return false;
        
        agentBrain.MovementController.NavigateTo(detectedSword.GlobalPosition);

        if(agentBrain.MovementController.NavigationAgent.IsNavigationFinished())
        {
            detectedSword.QueueFree();
            GoapStateUtils.SetState(worldState, "sword_equipped", true);
            return true;
        }

        return false;
    }

    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        return worldState;
    }

}
