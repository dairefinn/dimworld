namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class PatrolPath : GoapAction
{

    [Export] public InventoryItem swordItem;


    private int CurrentPointIndex { get; set; } = 0;

    private bool actionStarted = false;
    private Array<Vector2> patrolPath = null;

    /// <summary>
    /// Checks if the agent has a patrol path set in its world state and tries to find the path in the scene.
    /// Also checks if the agent has a sword in their inventory.
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <returns></returns>
    public override bool CheckProceduralPrecondition(IGoapAgent goapAgent)
    {
        if (goapAgent is not CharacterController characterController) return false; // Must be a character

        // Check if the agent has a patrol path set in its world state
        patrolPath = GetPatrolPath(goapAgent);
        if (patrolPath == null)
        {
            Preconditions.Add("patrol_path", Variant.Operator.Not.Equals(null));
            return false;
        }

        // Check if the agent has a sword in their inventory
        // TODO: This is checked by "has_item" in the agent's world state. If we want to put it here, we need to update the static conditions of this action
        // if (characterController.Inventory == null) return false;
        // GD.Print($"Checking if agent has sword: {characterController.Inventory.HasItem(swordItem.Id)}");
        // if (!characterController.Inventory.HasItem(swordItem.Id))
        // {
        //     Preconditions.Add("has_item", "item_sword");
        //     return false;
        // }

        return true;
    }

    public override bool Perform(IGoapAgent goapAgent, Dictionary<string, Variant> worldState, double delta)
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
    /// <param name="goapAgent"></param>
    /// <returns></returns>
    private static Array<Vector2> GetPatrolPath(IGoapAgent goapAgent)
    {
        // Check if the agent has a set patrol path stored
        NodePath nodePath = goapAgent.WorldState["patrol_path"].AsNodePath();
        if (nodePath == null) return null;

        // Check if the noed is a valid Path2D node in the scene
        if (goapAgent is not Node2D node2D) return null;
        Path2D patrolPath = node2D.GetNodeOrNull<Path2D>(nodePath);
        if (!IsInstanceValid(patrolPath)) return null;
        if (patrolPath.Curve.PointCount == 0) return null;

        // Get the points of the path as an array of Vector2s
        Array<Vector2> points = [];
        for (int i = 0; i < patrolPath.Curve.PointCount; i++)
        {
            Vector2 pointLocal = patrolPath.Curve.GetPointPosition(i);
            Vector2 pointGlobal = patrolPath.GlobalPosition + pointLocal;
            points.Add(pointGlobal);
        }

        // Make sure the path has at least one point
        if (points.Count == 0) return null;

        return points;
    }

}
