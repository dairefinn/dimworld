namespace Dimworld;

using System;
using System.Linq;
using Godot;

public partial class PatrolPath : GoapAction
{

    [Export] public string PathName { get; set; }

    public override bool Perform(AgentController agent, double delta)
    {
        Polygon2D Path = agent.GetTree().GetNodesInGroup("patrol_paths").OfType<Polygon2D>().Where(p => p.Name == PathName).FirstOrDefault();

        if (Path == null)
        {
            GD.Print("No paths found");
            return false;
        }

        Vector2 nearestPoint = GetNextPointOnPath(agent, Path);
        agent.NavigateTo(nearestPoint);

        return true;
    }

    private Vector2 GetNextPointOnPath(AgentController agent, Polygon2D path)
    {
        Vector2 currentPosition = agent.GlobalPosition;
        Vector2[] points = path.Polygon;
        Vector2 currentPoint = agent.NavigationAgent.TargetPosition;

        // If CurrentPoint is not on the path, find the nearest point on the path
        if (!points.Contains(currentPoint))
        {
            GD.Print("Current point is not on the path");
            Vector2 nearestPoint = points[0];
            float minDistance = float.MaxValue;
            foreach (Vector2 point in points)
            {
                float distance = currentPosition.DistanceTo(point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }
            
            return nearestPoint;
        }

        GD.Print("Finding next point on path");
        // Otherwise, find the next point on the path
        int currentPointIndex = Array.IndexOf(points, currentPoint);
        int nextPointIndex = currentPointIndex + 1;
        if (nextPointIndex >= points.Length)
        {
            nextPointIndex = 0;
        }
        Vector2 nextPoint = points[nextPointIndex];

        return nextPoint;
    }

}
