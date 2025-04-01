namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    [Export] public InventoryItem swordItem;


    private int CurrentPointIndex { get; set; } = 0;

    private bool actionStarted = false;


    public override bool CheckProceduralPrecondition(CharacterController characterController)
    {
        Array<Vector2> patrolPath = GetPatrolPath(characterController);
        return patrolPath != null;
    }

    public override bool Perform(CharacterController characterController, Dictionary<string, Variant> worldState, double delta)
    {
        Array<Vector2> patrolPath = GetPatrolPath(characterController);
        CurrentPointIndex = GetNextPointOnPath(characterController, patrolPath);
        Vector2 currentPoint = patrolPath[CurrentPointIndex];
        characterController.NavigateTo(currentPoint);

        if (!actionStarted)
        {
            characterController.Say("I'm starting my patrol.");
        }
        actionStarted = true;

        return false; // Always return false so the agent will continue to patrol
    }

    private int GetNextPointOnPath(CharacterController agent, Array<Vector2> points)
    {
        Vector2 currentPosition = agent.GlobalPosition;
        Vector2 currentTarget = agent.NavigationAgent.TargetPosition;

        // If CurrentPoint is not on the path, find the nearest point on the path
        if (!points.Contains(currentTarget))
        {
            int nearestPointIndex = 0;
            float minDistance = float.MaxValue;
            for(int index = 0; index < points.Count; index++)
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
        if (nextPointIndex >= points.Count)
        {
            nextPointIndex = 0;
        }
        return nextPointIndex;
    }

    private static Array<Vector2> GetPatrolPath(CharacterController characterController)
    {
        // Check if the agent has a set patrol path stored
        NodePath nodePath = characterController.WorldState["patrol_path"].AsNodePath();
        if (nodePath == null) return null;

        Node node = characterController.GetNodeOrNull(nodePath);
        if (!IsInstanceValid(node)) return null;

        Path2D patrolPath = (Path2D)node;
        if (patrolPath.Curve.PointCount == 0) return null;

        Array<Vector2> points = [];
        for (int i = 0; i < patrolPath.Curve.PointCount; i++)
        {
            Vector2 pointLocal = patrolPath.Curve.GetPointPosition(i);
            Vector2 pointGlobal = patrolPath.GlobalPosition + pointLocal;
            points.Add(pointGlobal);
        }
        if (points.Count == 0) return null;

        return points;
    }

}
