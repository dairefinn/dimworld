namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class GoapPlanner : Node
{

    private class GoapPlanFinder
    {

        private readonly GoapAction[] currentActions;

        public GoapPlanFinder(GoapAction[] initialActions)
        {
            currentActions = [.. initialActions];
        }

        /// <summary>
        /// Gets a plan to achieve the goal set from the current world state.
        /// </summary>
        /// <param name="worldState"></param>
        /// <param name="goalSet"></param>
        /// <returns></returns>
        public GoapAction[] GetPlan(GoapGoal goal, Dictionary<string, Variant> worldState)
        {
            if (goal == null) return [];
            
            // Duplicate desired state of the highest priority goal
            Dictionary<string, Variant> desiredState = GoapStateUtils.Duplicate(goal.DesiredState);
            if (desiredState.Count == 0) return [];
            GoapStateUtils.PrintState(desiredState, "Desired State");

            GoapPlanNode planTree = GetPossiblePlans(goal, desiredState, worldState);
            // LogPlanTree(planTree);

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
        // TODO: This will infinite loop if an agent can both turn off and turn on the lights
        public GoapPlanNode GetPossiblePlans(GoapGoal goal, Dictionary<string, Variant> desiredState, Dictionary<string, Variant> worldState)
        {
            GoapPlanNode planNode = new GoapPlanNode
            {
                Action = null,
                State = worldState,
                Children = []
            };

            foreach (GoapAction action in currentActions)
            {
                if (GoapStateUtils.IsSubsetOf(action.Preconditions, worldState))
                {
                    Dictionary<string, Variant> newState = GoapStateUtils.Duplicate(worldState);
                    GoapStateUtils.Add(newState, action.Effects);

                    GoapPlanNode childNode = GetPossiblePlans(goal, desiredState, newState);
                    if (childNode != null)
                    {
                        childNode.Action = action;
                        planNode.Children.Add(childNode);
                    }
                }
            }

            return planNode;
        }

        /// <summary>
        /// Recursively finds the best plan to achieve the goal from the plan tree
        /// </summary>
        /// <param name="goapPlanNode">The root node of the action tree</param>
        /// <returns></returns>
        private GoapAction[] FindBestPlan(GoapPlanNode goapPlanNode)
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

        public class GoapPlanNode
        {
            public GoapAction Action { get; set; }
            public Dictionary<string, Variant> State { get; set; }
            public System.Collections.Generic.List<GoapPlanNode> Children { get; set; }
        }

        private void LogPlanTree(GoapPlanNode rootNode, int depth = 0)
        {
            string indent = "";
            for (int i = 0; i < depth; i++)
            {
                indent += "  ";
            }

            GD.Print(indent + rootNode.Action?.Name);
            foreach (GoapPlanNode child in rootNode.Children)
            {
                LogPlanTree(child, depth + 1);
            }
        }

    }

    /// <summary>
    /// Figures out what actions the agent should take to satisfy their goal set given the current world state.
    /// </summary>
    /// <param name="worldState">The current state of the agent's world</param>
    /// <param name="actionSet">The actions the agent can perform</param>
    /// <param name="goalSet">The goals the agent wants to achieve</param>
    /// <returns>A of[]actions the agent should attempt to perform</returns>
    public static GoapAction[] GetPlan(GoapGoal goal, Dictionary<string, Variant> worldState, GoapAction[] actionSet)
    {
        GD.Print("Getting plan");

        if (actionSet.Length == 0) {
            GD.Print("No actions to plan with");
            return [];
        }

        GD.Print("Planning for goal " + goal.Name + " with " + actionSet.Length + " actions");
        GoapStateUtils.PrintState(worldState, "World State");

        GoapPlanFinder planFinder = new(actionSet);

        return planFinder.GetPlan(goal, worldState);
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
            if (goal.IsValid() && (bestGoal == null || goal.Priority > bestGoal.Priority))
            {
                bestGoal = goal;
            }
        }

        return bestGoal;
    }

}