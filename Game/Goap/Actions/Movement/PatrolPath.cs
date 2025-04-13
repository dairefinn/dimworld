namespace Dimworld.GOAP.Actions;

using System.Threading;
using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.GOAP;
using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    private Array<Vector2> patrolPath = null;
    private int CurrentPointIndex { get; set; } = 0;
    private bool actionStarted = false;

    private Path2D patrolPathNode = null;
    private Vector2 patrolPathNodeGlobalPosition = Vector2.Zero;


    public override GoapState GetPreconditions()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"has_items_equipped", new Array<Variant> { "item-sword" }},
        });
    }

    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"current_action", "patrol"}
        });
    }

    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
    {
        patrolPath = GetPatrolPath(goapAgent);
        if (patrolPath.Count == 0) return false; // Must have at least one point in the patrol path

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not IHasNavigation hasNavigation) return false;
        if (patrolPath == null) return false; // Must have a patrol path
        if (patrolPath.Count == 0) return false; // Must have at least one point in the patrol path

        CurrentPointIndex = GetNextPointOnPath(goapAgent, hasNavigation, patrolPath);

        Vector2 currentPoint = patrolPath[CurrentPointIndex];
        hasNavigation.NavigateTo(currentPoint);

        if (!actionStarted && goapAgent is ICanSpeak canSpeak)
        {
            canSpeak.Say("I'm starting my patrol.");
        }
        actionStarted = true;

        return false; // Always return false so the agent will continue to patrol
    }

    /// <summary>
    /// Gets the next point on the patrol path. If the current point has not been reached, it returns the current point index.
    /// </summary>
    /// <param name="hasNavigation">The character controller that is patrolling</param>
    /// <param name="points">The points of the patrol path</param>
    /// <returns>The index of the next point on the patrol path or the nearest point if not currently patrolling</returns>
    private int GetNextPointOnPath(IGoapAgent goapAgent, IHasNavigation hasNavigation, Array<Vector2> points)
    {
        Vector2 currentPosition = goapAgent.GlobalPositionThreadSafe;
        Vector2 currentTarget = hasNavigation.GetTargetPosition();

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
        if (!hasNavigation.IsTargetReached())
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

    /// <summary>
    /// Looks for a patrol_path entry in the agent's world state and then tries to find the path in the scene.
    /// If the path is found, it returns the points of the path as an array of Vector2s.
    /// If the path is not found, it returns null.
    /// </summary>
    /// <param name="goapAgent">The agent that is trying to find the path</param>
    /// <returns></returns>
    private Array<Vector2> GetPatrolPath(IGoapAgent goapAgent)
    {
        if (goapAgent == null) return [];
        if (goapAgent.WorldState == null) return [];
        if (!goapAgent.WorldState.ContainsKey("patrol_path")) return [];

        // Check if the agent has a set patrol path stored
        NodePath nodePath = goapAgent.WorldState.GetKey("patrol_path").AsNodePath();
        if (nodePath == null) return [];

        // Check if the agent is a valid Node2D
        if (goapAgent is not Node2D node2D) return [];

        // Use a thread-safe mechanism to get the patrol path
        CallDeferred(MethodName.FetchPatrolPath, [node2D, nodePath]);

        // Wait for the deferred call to complete
        while (patrolPathNode == null)
        {
            Thread.Sleep(10); // Sleep briefly to avoid busy-waiting
        }

        if (!IsInstanceValid(patrolPathNode)) return [];
        if (patrolPathNode.Curve.PointCount == 0) return [];

        // Get the points of the path as an array of Vector2s
        Array<Vector2> points = [];
        for (int i = 0; i < patrolPathNode.Curve.PointCount; i++)
        {
            Vector2 pointLocal = patrolPathNode.Curve.GetPointPosition(i);
            Vector2 pointGlobal = patrolPathNodeGlobalPosition + pointLocal;
            points.Add(pointGlobal);
        }

        // Make sure the path has at least one point
        if (points.Count == 0) return [];

        return points;
    }

    private void FetchPatrolPath(Node2D agentNode2D, NodePath nodePath)
    {
        patrolPathNode = agentNode2D.GetNodeOrNull<Path2D>(nodePath);
        patrolPathNodeGlobalPosition = patrolPathNode.GlobalPosition;
    }

}
