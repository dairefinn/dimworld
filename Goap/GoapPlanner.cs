namespace Dimworld.GOAP;

using System.Linq;
using Dimworld.Developer;
using Godot.Collections;


public class GoapPlanner
{

    private static readonly bool PrintDebugLogs = false; // TODO: Wire this up to a debug menu


    private static string GetIndent(int depth, string actionName)
    {
        string indent = "";
        for (int i = 0; i < depth; i++)
        {
            indent += "\t";
        }
        return indent + "[" + actionName + "] ";
    }

    private static void PrintDebug(string message)
    {
        if (!PrintDebugLogs) return;
        // DeveloperConsole.AddConsoleEntry("[GoapPlanner] " + message);
        DeveloperConsole.Print(message);
    }


    /// <summary>
    /// Gets the goal sets in order of priority. The first goal in the array is the most important goal.
    /// </summary>
    /// <param name="goalSet">The original goal set</param>
    /// <returns>The sorted goal set</returns>
    public static Array<GoapGoal> GetGoalsInOrder(Array<GoapGoal> goalSet)
    {
        if (goalSet.Count == 0) return [];

        return [
            ..goalSet
            .OrderByDescending(goal => goal.Priority)
            .ToArray()
        ];
    }

    /// <summary>
    /// Figures out what actions the agent should take to satisfy their goal set given the current world state.
    /// </summary>
    /// <param name="goal">The goal the agent is trying to achieve</param>
    /// <param name="goapAgent">The agent that is trying to achieve the goal</param>
    /// <returns>An array of actions the agent should attempt to perform</returns>
    public static GoapAction[] GetPlan(GoapGoal goal, IGoapAgent goapAgent)
    {
        PrintDebug("========== Getting plan for goal " + goal.Name + " ==========");
        if (goal == null) return [];
        if (goapAgent.ActionSet.Count == 0) return [];

        GoapPlanNode planTree = GetPossiblePlans(goal, goapAgent);
        PrintDebug("\n" + DrawTree(planTree));

        GoapAction[] plan = FindBestPlan(planTree);

        return plan;
    }

    /// <summary>
    /// Builds a graph of possible plans to achieve the goal
    /// </summary>
    /// <param name="goal">The goal the agent is trying to achieve</param>
    /// <param name="goapAgent">The agent that is trying to achieve the goal</param>
    /// <returns>The root node of the plan tree</returns>
    private static GoapPlanNode GetPossiblePlans(GoapGoal goal, IGoapAgent goapAgent)
    {
        // Create the root node - we don't use this node directly but it's children are all the possible plans we can use
        GoapPlanNode rootNode = new()
        {
            Action = null,
            Cost = 0,
            Children = []
        };

        // Check each possible action to see if it can be performed to achieve the goal
        foreach(GoapAction action in goapAgent.ActionSet)
        {
            GoapPlanNode childNode = BuildPlanTree(goal.DesiredState, goapAgent.WorldState, goapAgent.ActionSet, goapAgent, action, action.Cost);
            if (childNode != null)
            {
                rootNode.Children.Add(childNode);
            }
        }

        // Return null if no valid plans are found
        return rootNode;
    }

    private static GoapPlanNode BuildPlanTree(GoapState desiredState, GoapState worldState, Array<GoapAction> possibleActions, IGoapAgent goapAgent, GoapAction currentAction, double accumulatedCost, int depth = 0)
    {
        GoapAction currentActionCopy = currentAction.Duplicate() as GoapAction;
        currentActionCopy.PreEvaluate(worldState, desiredState);

        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "Current desired state: " + desiredState?.ToString());
        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "Current action effects: " + currentActionCopy.Effects.ToString());

        // If this action does not satisfy the desired state, it's not a valid plan
        if (!currentActionCopy.CanSatisfy(desiredState)) {
            PrintDebug(GetIndent(depth, currentActionCopy.Name) + "[Invalid] Action " + currentActionCopy.Name + " WILL NOT satisfy the desired state");
            return null;
        }

        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "Action " + currentActionCopy.Name + " will satisfy the desired state");

        // Create a node for this action
        GoapPlanNode currentNode = new()
        {
            Action = currentActionCopy,
            Cost = accumulatedCost,
            Children = []
        };

        // If we can perform the action, then this is a valid plan
        if (currentActionCopy.CanPerform(goapAgent, worldState))
        {
            return currentNode;
        }

        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "Action " + currentActionCopy.Name + " CANNOT be performed");
        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "Preconditions:" + currentActionCopy.Preconditions.ToString());

        // Otherwise, figure out how to satisfy the preconditions of the action
        GoapState desiredStateForAction = currentActionCopy.Preconditions.Duplicate() as GoapState;

        // Check each of the remaining actions recursively to see if they can satisfy the preconditions of this action
        GoapAction[] remainingActions = possibleActions.Where(action => action != currentActionCopy).ToArray();
        foreach(GoapAction nextAction in remainingActions)
        {
            GoapAction actionCopy = nextAction.Duplicate() as GoapAction;

            PrintDebug(GetIndent(depth + 1, currentActionCopy.Name) + "Checking if " + actionCopy.Name + " can satisfy the preconditions of " + currentActionCopy.Name);
            GoapPlanNode childNode = BuildPlanTree(desiredStateForAction, worldState, possibleActions[1..], goapAgent, actionCopy, accumulatedCost + actionCopy.Cost, depth + 1);
            if (childNode != null)
            {
                currentNode.Cost += childNode.Cost;
                currentNode.Children.Add(childNode);
            }
        }

        // If this action has any actions that can satisfy its preconditions, then this is a valid plan
        if (currentNode.Children.Count > 0) {
            PrintDebug(GetIndent(depth, currentActionCopy.Name) + "[Valid] Action " + currentActionCopy.Name + " can be performed by satisfying its preconditions");
            return currentNode;
        }

        // Otherwise, this is not a valid plan
        PrintDebug(GetIndent(depth, currentActionCopy.Name) + "[Invalid] No actions can satisfy the preconditions of " + currentActionCopy.Name);
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
        if (rootNode.Children.Count == 0) return "";
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

}