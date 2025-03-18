namespace Dimworld;

using System;
using System.Linq;
using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    private int CurrentPointIndex { get; set; } = 0;

    public override bool CheckProceduralPrecondition(AgentBrain agentBrain)
    {
        if (agentBrain.PatrolPath == null) return false;
        if (agentBrain.PatrolPath.Count == 0) return false;
        return true;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        Vector2[] points = [..agentBrain.PatrolPath.Select(p => p.GlobalPosition)];
        CurrentPointIndex = GetNextPointOnPath(agentBrain.Agent, points);
        Vector2 currentPoint = points[CurrentPointIndex];
        agentBrain.Agent.NavigateTo(currentPoint);

        return false; // Always return false so the agent will continue to patrol
    }

    private int GetNextPointOnPath(AgentController agent, Vector2[] points)
    {
        Vector2 currentPosition = agent.GlobalPosition;
        Vector2 currentTarget = agent.NavigationAgent.TargetPosition;

        // If CurrentPoint is not on the path, find the nearest point on the path
        if (!points.Contains(currentTarget))
        {
            GD.Print("Current point is not on the path");
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
        GD.Print("Finding next point on path");
        int nextPointIndex = CurrentPointIndex + 1;
        if (nextPointIndex >= points.Length)
        {
            nextPointIndex = 0;
        }
        return nextPointIndex;
    }

}
