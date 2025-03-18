namespace Dimworld;

using System;
using System.Linq;
using Godot;
using Godot.Collections;


public partial class GoapPlanner : Node
{

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
        if (goal == null) return [];
        if (actionSet.Length == 0) return [];

        // Duplicate desired state of the highest priority goal
        Dictionary<string, Variant> desiredState = GoapStateUtils.Duplicate(goal.DesiredState);
        if (desiredState.Count == 0) return [];

        GoapPlanNode planTree = GetPossiblePlans(goal, actionSet, desiredState, worldState, agentBrain);
        // GD.Print(DrawTree(planTree));

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
    private static GoapPlanNode GetPossiblePlans(GoapGoal goal, GoapAction[] possibleActions, Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState, AgentBrain agentBrain)
    {
        // Create the root node - we don't use this node directly but it's children are all the possible plans we can use
        GoapPlanNode rootNode = new()
        {
            Action = null,
            Cost = 0,
            Children = []
        };

        // Check if the desired state is already satisfied
        // if (goal.IsSatisfied(worldState, agentBrain)) {
        //     // GD.Print("desired state already satisfied");
        //     return rootNode;
        // }

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

    private static GoapPlanNode BuildPlanTree(Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState, GoapAction[] possibleActions, AgentBrain agentBrain, GoapAction currentAction, double accumulatedCost)
    {
        // If this action does not satisfy the desired state, it's not a valid plan
        if (!GoapStateUtils.IsSubsetOf(currentAction.Effects, desiredState)) {
            // GD.Print("[Invalid] Action " + currentAction.Name + " WILL NOT satisfy the desired state");
            return null;
        }

        // GD.Print("Action " + currentAction.Name + " will satisfy the desired state");

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
            // GD.Print("[Invalid] Action " + currentAction.Name + " cannot be performed because procedural preconditions are not satisfied");
            return null;
        }

		bool staticPreconditionsSatisfied = currentAction.CheckStaticPreconditions(worldState);
        if (staticPreconditionsSatisfied) {
            // GD.Print("[Valid] Action " + currentAction.Name + " can be performed");
            return currentNode;
        }

        // GD.Print("Action " + currentAction.Name + " CANNOT be performed");

        // Otherwise, figure out how to satisfy the preconditions of the action
        Dictionary<string, Variant> desiredStateForAction = GoapStateUtils.Duplicate(currentAction.Preconditions);

        // Check each of the remaining actions recursively to see if they can satisfy the preconditions of this action
        GoapAction[] remainingActions = possibleActions.Where(action => action != currentAction).ToArray();
        foreach(GoapAction nextAction in remainingActions)
        {
            // GD.Print("Checking if " + nextAction.Name + " can satisfy the preconditions of " + currentAction.Name);
            GoapPlanNode childNode = BuildPlanTree(desiredStateForAction, worldState, possibleActions[1..], agentBrain, nextAction, accumulatedCost + nextAction.Cost);
            if (childNode != null)
            {
                currentNode.Cost += childNode.Cost;
                currentNode.Children.Add(childNode);
            }
        }

        // If this action has any actions that can satisfy its preconditions, then this is a valid plan
        if (currentNode.Children.Count > 0) {
            // GD.Print("[Valid] Action " + currentAction.Name + " can be performed by satisfying its preconditions");
            return currentNode;
        }

        // Otherwise, this is not a valid plan
        // GD.Print("[Invalid] No actions can satisfy the preconditions of " + currentAction.Name);
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
        public double Cost { get; set; }
        public System.Collections.Generic.List<GoapPlanNode> Children { get; set; }
    }
}