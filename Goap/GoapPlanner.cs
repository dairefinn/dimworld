namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class GoapPlanner : Node
{

    /// <summary>
    /// Figures out what actions the agent should take to satisfy their goal set given the current world state.
    /// </summary>
    /// <param name="worldState">The current state of the agent's world</param>
    /// <param name="actionSet">The actions the agent can perform</param>
    /// <param name="goalSet">The goals the agent wants to achieve</param>
    /// <returns>A of[]actions the agent should attempt to perform</returns>
    public static GoapAction[] GetPlan(GoapGoal goal, Dictionary<string, Variant> worldState, GoapAction[] actionSet)
    {
        if (actionSet.Length == 0) {
            GD.Print("No actions to plan with");
            return [];
        }

        return GetPlan(goal, actionSet, worldState);
    }

    /// <summary>
    /// Gets the highest priority goal that can be achieved with the current world state.
    /// </summary>
    /// <param name="goalSet">The set of goals to choose from</param>
    /// <returns></returns>
    public static GoapGoal GetBestGoal(GoapGoal[] goalSet, Dictionary<string, Variant> worldState)
    {
        if (goalSet.Length == 0) return null;

        GoapGoal bestGoal = null;
        foreach (GoapGoal goal in goalSet)
        {
            if (goal.IsSatisfied(worldState)) continue;
            if (!goal.IsValid()) continue;
            if (bestGoal == null || goal.Priority > bestGoal.Priority)
            {
                bestGoal = goal;
            }
        }

        return bestGoal;
    }

    /// <summary>
    /// Gets a plan to achieve the goal set from the current world state.
    /// </summary>
    /// <param name="worldState"></param>
    /// <param name="goalSet"></param>
    /// <returns></returns>
    private static GoapAction[] GetPlan(GoapGoal goal, GoapAction[] possibleActions, Dictionary<string, Variant> worldState)
    {
        if (goal == null) return [];
        
        // Duplicate desired state of the highest priority goal
        Dictionary<string, Variant> desiredState = GoapStateUtils.Duplicate(goal.DesiredState);
        if (desiredState.Count == 0) return [];

        GoapPlanNode planTree = GetPossiblePlans(goal, possibleActions, desiredState, worldState);
        GD.Print(DrawTree(planTree));

        GoapAction[] plan = FindBestPlan(planTree);

        return plan;
    }

    /// <summary>
    /// Builds a graph of possible plans to achieve the goal
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="desiredState"></param>
    /// <param name="worldState"></param>
    /// <returns></returns>
    private static GoapPlanNode GetPossiblePlans(GoapGoal goal, GoapAction[] possibleActions, Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState)
    {
        Dictionary<string, Variant> worldStateCopy = GoapStateUtils.Duplicate(worldState);

        // Check if the desired state is already satisfied
        if (GoapStateUtils.IsSubsetOf(desiredState, worldStateCopy))
        {
            return new GoapPlanNode
            {
                Action = null,
                State = worldStateCopy,
                Children = []
            };
        }

        // Root node - Not really used for anything but allows us to build a tree of possible action combinations
        GoapPlanNode planNode = new()
        {
            Action = null,
            State = worldStateCopy,
            Children = []
        };

        // Check each possible action to see if it can be performed to achieve the goal
        foreach (GoapAction action in possibleActions)
        {
            // Check if the actions effects result in the desired state
            if (GoapStateUtils.IsSubsetOf(action.Effects, desiredState))
            {
                // Check if the action can be performed with the current world state
                if (GoapStateUtils.IsSubsetOf(action.Preconditions, worldStateCopy))
                {
                    // Create a new world state with the action's effects
                    Dictionary<string, Variant> newWorldState = GoapStateUtils.Add(worldStateCopy, action.Effects);

                    // Recursively find the next best action to achieve the goal
                    GoapPlanNode childNode = GetPossiblePlans(goal, possibleActions, desiredState, newWorldState);
                    if (childNode != null)
                    {
                        childNode.Action = action;
                        planNode.Children.Add(childNode);
                    }
                }
            }
        }

        // Return null if no valid plans are found
        return planNode.Children.Count > 0 ? planNode : null;
    }

    /// <summary>
    /// Recursively finds the best plan to achieve the goal from the plan tree
    /// </summary>
    /// <param name="goapPlanNode">The root node of the action tree</param>
    /// <returns></returns>
    private static GoapAction[] FindBestPlan(GoapPlanNode goapPlanNode)
    {
        if (goapPlanNode.Children.Count == 0)
        {
            return [goapPlanNode.Action];
        }

        GoapAction[] bestPlan = [];
        foreach (GoapPlanNode child in goapPlanNode.Children)
        {
            GoapAction[] plan = FindBestPlan(child);
            if (plan.Length > bestPlan.Length)
            {
                bestPlan = plan;
            }
        }

        if (goapPlanNode.Action != null)
        {
            System.Collections.Generic.List<GoapAction> planList = [.. bestPlan];
            planList.Insert(0, goapPlanNode.Action);
            bestPlan = planList.ToArray();
        }

        return bestPlan;
    }

    /// <summary>
    /// Generates a pretty tree diagram of the plan tree for logging later.
    /// Needs to return a string because Godot can only print entire lines at a time and cannot append to a line.
    /// </summary>
    /// <param name="rootNode"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private static string DrawTree(GoapPlanNode rootNode, int depth = 0)
    {
        if (rootNode == null) return "";
        string tree = "";
        for (int i = 0; i < depth; i++)
        {
            tree += "  ";
        }
        tree += rootNode.Action != null ? rootNode.Action.Name : "Root";
        tree += "\n";

        foreach (GoapPlanNode child in rootNode.Children)
        {
            tree += DrawTree(child, depth + 1);
        }

        return tree;
    }

    private class GoapPlanNode
    {
        public GoapAction Action { get; set; }
        public Dictionary<string, Variant> State { get; set; }
        public System.Collections.Generic.List<GoapPlanNode> Children { get; set; }
    }
}