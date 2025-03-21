namespace Dimworld;

using System;
using System.Linq;
using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    [Export] public InventoryItem swordItem;


    private int CurrentPointIndex { get; set; } = 0;


    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        // Check if the agent has a set patrol path stored
        Array<NodePath> patrolPath = (Array<NodePath>)GoapStateUtils.GetState(agentBrain.WorldState, "patrol_path", new Array<NodePath>());
        if (patrolPath == null) return false;
        if (patrolPath.Count == 0) return false;

        return true;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        Array<NodePath> nodeReferences = (Array<NodePath>)GoapStateUtils.GetState(agentBrain.WorldState, "patrol_path", new Array<NodePath>());
        Array<Marker2D> patrolPath = [];
        foreach (NodePath nodeReference in nodeReferences)
        {
            Node node = agentBrain.GetNodeOrNull(nodeReference);
            if (node is Marker2D marker)
            {
                patrolPath.Add(marker);
            }
        }
        Vector2[] points = [..patrolPath.Select(p => p.GlobalPosition)];
        CurrentPointIndex = GetNextPointOnPath(agentBrain.MovementController, points);
        Vector2 currentPoint = points[CurrentPointIndex];
        agentBrain.MovementController.NavigateTo(currentPoint);

        return false; // Always return false so the agent will continue to patrol
    }

    private int GetNextPointOnPath(MovementController agent, Vector2[] points)
    {
        Vector2 currentPosition = agent.GlobalPosition;
        Vector2 currentTarget = agent.NavigationAgent.TargetPosition;

        // If CurrentPoint is not on the path, find the nearest point on the path
        if (!points.Contains(currentTarget))
        {
            int nearestPointIndex = 0;
            float minDistance = float.MaxValue;
            for(int index = 0; index < points.Length; index++)
            {
                Vector2 point = points[index];
                float distance = currentPosition.DistanceTo(point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPointIndex = index;
                }
            }

            return nearestPointIndex;
        }

        // If the current point has not been reached, continue to the current point
        if (!agent.NavigationAgent.IsNavigationFinished())
        {
            return CurrentPointIndex;
        }

        // Otherwise, find the next point on the path
        int nextPointIndex = CurrentPointIndex + 1;
        if (nextPointIndex >= points.Length)
        {
            nextPointIndex = 0;
        }
        return nextPointIndex;
    }

}
