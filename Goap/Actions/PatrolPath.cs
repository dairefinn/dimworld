namespace Dimworld;

using System;
using System.Linq;
using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    [Export] public string PathName { get; set; }

    private Vector2[] Points { get; set; }
    private int CurrentPointIndex { get; set; } = 0;

    public override Dictionary<string, Variant> OnStart(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        Polygon2D Path = agentBrain.GetTree().GetNodesInGroup("patrol_paths").OfType<Polygon2D>().Where(p => p.Name == PathName).FirstOrDefault();

        if (Path == null)
        {
            GD.Print("No paths found");
            return worldState;
        }

        Points = Path.Polygon;
        CurrentPointIndex = 0;

        // TODO: The goal wants "is_patrolling" to be true but we want the agent to infinitely patrol until another goal takes priority so we should never set "is_patrolling" to anything. Not sure if this is the right way to handle this.
        // GoapStateUtils.SetState(worldState, "is_patrolling", true);

        return worldState;
    }

    public override bool Perform(AgentBrain agentBrain, Dictionary<string, Variant> worldState, double delta)
    {
        CurrentPointIndex = GetNextPointOnPath(agentBrain.Agent, Points);
        Vector2 currentPoint = Points[CurrentPointIndex];
        agentBrain.Agent.NavigateTo(currentPoint);

        return false; // Always return false so the agent will continue to patrol
    }

    public override Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
    {
        // GoapStateUtils.SetState(worldState, "is_patrolling", false);
        return worldState;
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
