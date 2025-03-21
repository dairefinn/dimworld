namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;


public class GoapPlanner
{

    private static readonly bool PrintDebugLogs = true;

    public static GoapGoal[] GetGoalsInOrder(GoapGoal[] goalSet, Dictionary<string, Variant> worldState, AgentBrain agentBrain)
    {
        if (goalSet.Length == 0) return [];

        return goalSet
            .OrderByDescending(goal => goal.Priority)
            .ToArray();
    }

    /// <summary>
    /// Figures out what actions the agent should take to satisfy their goal set given the current world state.
    /// </summary>
    /// <param name="worldState">The current state of the agent's world</param>
    /// <param name="actionSet">The actions the agent can perform</param>
    /// <param name="goalSet">The goals the agent wants to achieve</param>
    /// <returns>A of[]actions the agent should attempt to perform</returns>
    public static GoapAction[] GetPlan(GoapGoal goal, Dictionary<string, Variant> worldState, GoapAction[] actionSet, AgentBrain agentBrain)
    {
        PrintDebug("========== Getting plan for goal " + goal.Name + " ==========");
        if (goal == null) return [];
        if (actionSet.Length == 0) return [];

        // Duplicate desired state of the highest priority goal
        Dictionary<string, Variant> desiredState = GoapStateUtils.Duplicate(goal.DesiredState);
        if (desiredState.Count == 0) return [];


        GoapPlanNode planTree = GetPossiblePlans(actionSet, desiredState, worldState, agentBrain);
        PrintDebug("\n" + DrawTree(planTree));

        GoapAction[] plan = FindBestPlan(planTree);

        return plan;
    }

    /// <summary>
    /// Builds a graph of possible plans to achieve the goal
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="desiredState"></param>
    /// <param name="worldState"></param>
    /// <returns>The root node of the plan tree</returns>
    private static GoapPlanNode GetPossiblePlans(GoapAction[] possibleActions, Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState, AgentBrain agentBrain)
    {
        // Create the root node - we don't use this node directly but it's children are all the possible plans we can use
        GoapPlanNode rootNode = new()
        {
            Action = null,
            Cost = 0,
            Children = []
        };

        // Check each possible action to see if it can be performed to achieve the goal
        foreach(GoapAction action in possibleActions)
        {
            GoapPlanNode childNode = BuildPlanTree(desiredState, worldState, possibleActions, agentBrain, action, action.Cost);
            if (childNode != null)
            {

                rootNode.Children.Add(childNode);
            }
        }

        // Return null if no valid plans are found
        return rootNode;
    }

    private static GoapPlanNode BuildPlanTree(Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState, GoapAction[] possibleActions, AgentBrain agentBrain, GoapAction currentAction, double accumulatedCost, int depth = 0)
    {
        PrintDebug(GetIndent(depth) + GoapStateUtils.GetAsString(currentAction.Effects, "Action effects"));

        // If this action does not satisfy the desired state, it's not a valid plan
        if (!GoapStateUtils.IsSubsetOf(currentAction.Effects, desiredState)) {
            PrintDebug(GetIndent(depth) + "[Invalid] Action " + currentAction.Name + " WILL NOT satisfy the desired state");
            return null;
        }

        PrintDebug(GetIndent(depth) + "Action " + currentAction.Name + " will satisfy the desired state");

        // Create a node for this action
        GoapPlanNode currentNode = new()
        {
            Action = currentAction,
            Cost = accumulatedCost,
            Children = []
        };

        // If we can perform the action, then this is a valid plan
		bool proceduralPreconditionsSatisfied = currentAction.CheckProceduralPrecondition(agentBrain);
        if (!proceduralPreconditionsSatisfied) 
        {
            PrintDebug(GetIndent(depth) + "[Invalid] Action " + currentAction.Name + " cannot be performed because procedural preconditions are not satisfied");
            return null;
        }

		bool staticPreconditionsSatisfied = currentAction.CheckStaticPreconditions(worldState);
        if (staticPreconditionsSatisfied) {
            PrintDebug(GetIndent(depth) + "[Valid] Action " + currentAction.Name + " can be performed");
            return currentNode;
        }

        PrintDebug(GetIndent(depth) + "Action " + currentAction.Name + " CANNOT be performed");
        PrintDebug(GetIndent(depth) + GoapStateUtils.GetAsString(currentAction.Preconditions, "Preconditions"));

        // Otherwise, figure out how to satisfy the preconditions of the action
        Dictionary<string, Variant> desiredStateForAction = GoapStateUtils.Duplicate(currentAction.Preconditions);

        // Check each of the remaining actions recursively to see if they can satisfy the preconditions of this action
        GoapAction[] remainingActions = possibleActions.Where(action => action != currentAction).ToArray();
        foreach(GoapAction nextAction in remainingActions)
        {
            PrintDebug(GetIndent(depth) + "Checking if " + nextAction.Name + " can satisfy the preconditions of " + currentAction.Name);
            GoapPlanNode childNode = BuildPlanTree(desiredStateForAction, worldState, possibleActions[1..], agentBrain, nextAction, accumulatedCost + nextAction.Cost, depth + 1);
            if (childNode != null)
            {
                currentNode.Cost += childNode.Cost;
                currentNode.Children.Add(childNode);
            }
        }

        // If this action has any actions that can satisfy its preconditions, then this is a valid plan
        if (currentNode.Children.Count > 0) {
            PrintDebug(GetIndent(depth) + "[Valid] Action " + currentAction.Name + " can be performed by satisfying its preconditions");
            return currentNode;
        }

        // Otherwise, this is not a valid plan
        PrintDebug(GetIndent(depth) + "[Invalid] No actions can satisfy the preconditions of " + currentAction.Name);
        return null;
    }

    /// <summary>
    /// Finds the best path through the plan tree based on the total cost of the actions.
    /// </summary>
    /// <param name="goapPlanNode">The root node of the action tree</param>
    /// <returns></returns>
    private static GoapAction[] FindBestPlan(GoapPlanNode goapPlanNode)
    {
        GoapAction[] bestPlan = [];

        // This stops the root node from being added to the plan
        if (goapPlanNode.Action != null)
        {
            bestPlan = [goapPlanNode.Action];
        }

        // If we're at a leaf node, return the action
        if (goapPlanNode.Children.Count == 0) return bestPlan;

        // Otherwise, find the best plan from the children
        GoapPlanNode lowestCostChild = goapPlanNode.Children[0];
        for(int i = 1; i < goapPlanNode.Children.Count; i++)
        {
            GoapPlanNode currentChild = goapPlanNode.Children[i];
            if (currentChild.Cost < lowestCostChild.Cost)
            {
                lowestCostChild = currentChild;
            }
        }
        
        // Return this action followed by the best plan from the children
        return FindBestPlan(lowestCostChild).Concat(bestPlan).ToArray();
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
        for (int i = 0; i <= depth; i++)
        {
            if (i == depth)
            {
                tree += "  └─";
            }
            else
            {
                tree += "    ";
            }
        }
        tree += rootNode.Action != null ? rootNode.Action.Name : "Goal";
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
        public double Cost { get; set; }
        public System.Collections.Generic.List<GoapPlanNode> Children { get; set; }
        // public bool IsValid { get; set; } TODO: Might want to add this just so we can log the decision tree and not the entire text based log history
    }

    private static string GetIndent(int depth)
    {
        string indent = "";
        for (int i = 0; i < depth; i++)
        {
            indent += "    ";
        }
        return indent;
    }

    private static void PrintDebug(string message)
    {
        if (!PrintDebugLogs) return;
        GD.Print("[GoapPlanner] " + message);
    }

}