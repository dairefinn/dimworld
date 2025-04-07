namespace Dimworld.GOAP.Actions;

using System.Threading;
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
        if (goapAgent is not CharacterController characterController) return false; // Must be a character

        // Get the full patrol path from the stored node reference
        patrolPath = GetPatrolPath(goapAgent);
        if (patrolPath == null) return false; // Must have a patrol path set

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, GoapState worldState, double delta)
    {
        if (goapAgent is not CharacterController characterController) return false;

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

    /// <summary>
    /// Gets the next point on the patrol path. If the current point has not been reached, it returns the current point index.
    /// </summary>
    /// <param name="characterController">The character controller that is patrolling</param>
    /// <param name="points">The points of the patrol path</param>
    /// <returns>The index of the next point on the patrol path or the nearest point if not currently patrolling</returns>
    private int GetNextPointOnPath(CharacterController characterController, Array<Vector2> points)
    {
        Vector2 currentPosition = characterController.GlobalPosition;
        Vector2 currentTarget = characterController.NavigationAgent.TargetPosition;

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
        if (!characterController.NavigationAgent.IsNavigationFinished())
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
        // Check if the agent has a set patrol path stored
        NodePath nodePath = goapAgent.WorldState.GetKey("patrol_path").AsNodePath();
        if (nodePath == null) return null;

        // Check if the agent is a valid Node2D
        if (goapAgent is not Node2D node2D) return null;

        // Use a thread-safe mechanism to get the patrol path
        CallDeferred(MethodName.FetchPatrolPath, [node2D, nodePath]); //  (Path2D fetchedPath) => { patrolPath = fetchedPath; }

        // Wait for the deferred call to complete
        while (patrolPathNode == null)
        {
            Thread.Sleep(10); // Sleep briefly to avoid busy-waiting
        }

        if (!IsInstanceValid(patrolPathNode)) return null;
        if (patrolPathNode.Curve.PointCount == 0) return null;

        // Get the points of the path as an array of Vector2s
        Array<Vector2> points = [];
        for (int i = 0; i < patrolPathNode.Curve.PointCount; i++)
        {
            Vector2 pointLocal = patrolPathNode.Curve.GetPointPosition(i);
            Vector2 pointGlobal = patrolPathNodeGlobalPosition + pointLocal;
            points.Add(pointGlobal);
        }

        // Make sure the path has at least one point
        if (points.Count == 0) return null;

        return points;
    }

    private void FetchPatrolPath(Node2D agentNode2D, NodePath nodePath)//, Action<Path2D> callback)
    {
        patrolPathNode = agentNode2D.GetNodeOrNull<Path2D>(nodePath);
        patrolPathNodeGlobalPosition = patrolPathNode.GlobalPosition;
        // callback?.Invoke(patrolPath);
    }

}
