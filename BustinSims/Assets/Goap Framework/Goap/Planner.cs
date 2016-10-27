using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    public static class Planner {

        public enum DebugPlanning { None, Basics, Everything };

        /// <summary>
        /// Formulates a plan based on the given actions and goals
        /// </summary>
        /// <param name="actions">List of available actions</param>
        /// <param name="goals">List of available goals, these will be sorted by priority</param>
        /// <param name="currentGoal">If a plan is found, this will be the current goal</param>
        /// <returns>If a plan was found, a list of actions. Otherwise null</returns>
        public static List<Action> FormulatePlan(IGoap igoap, List<Action> actions, List<Goal> goals, out Goal currentGoal, DebugPlanning debug = DebugPlanning.None) {
            currentGoal = null;
            if (igoap == null || actions == null || actions.Count == 0 || goals == null || goals.Count == 0) {

                if (igoap == null) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable iGoap was found."); }
                if (actions == null || actions.Count == 0) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable actions for iGoap were found."); }
                if (goals == null || goals.Count == 0) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable goals for iGoap were found."); }

                return null;
            }

            foreach (Goal goal in goals.OrderByDescending(g => g.priority).ToList()) {
                List<Action> plan = FormulatePlan(igoap, actions, goal, out currentGoal, debug);

                if (plan != null)
                    return plan;
            }

            return null;
        }

        /// <summary>
        /// Formulates a plan based on the given actions and goal
        /// </summary>
        /// <param name="actions">List of available actions</param>
        /// <param name="goals">Available goal</param>
        /// <param name="currentGoal">If a plan is found, this will be the current goal</param>
        /// <returns>If a plan was found, a list of actions. Otherwise null</returns>
        public static List<Action> FormulatePlan(IGoap igoap, List<Action> actions, Goal goal, out Goal currentGoal, DebugPlanning debug = DebugPlanning.None) {
            currentGoal = goal;
            if (igoap == null || actions == null || actions.Count == 0 || goal == null) {
                if (igoap == null) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable iGoap was found."); }
                if (actions == null || actions.Count == 0) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable actions for iGoap were found."); }
                if (goal) { DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Can't formulate a plan because no suitable goal for iGoap was found."); }
            }

            DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Formulating a plan for goal {0}.", goal.GetType());

            goal.OnGoalSetup();
            if (!goal.IsGoalRelevant()) {
                DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Goal {0} is not relevant.", goal.GetType());
                return null;
            }

            List<Action> plan = FindSolution(igoap, actions, goal, debug);
            if (plan == null) DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Couldn't find a plan for goal {0}, returning null.", goal.GetType()); else DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Found a plan for goal {0} and returning it.", goal.GetType());

            return plan;
        }

        private static List<Action> FindSolution(IGoap igoap, List<Action> actions, Goal goal, DebugPlanning debug) {
            // Create the closed/ open list and add the goal state to the latter
            List<Node> closedList = new List<Node>();
            List<Node> openList = new List<Node>();
            openList.Add(new Node(null, null, goal.succes));

            DebugText(debug, DebugPlanning.Everything, "Starting state is {0}.", ConditionsToText(goal.succes));

            // As long as there are nodes in the open list, we continue
            while (true) {
                Node p = GetCheapestNode(openList);
                if (p.action != null) DebugText(debug, DebugPlanning.Everything, "Picked open node with action {0} from the open list.", p.action.GetType());

                // Look for each action if it gives the desired effects
                foreach (Action action in actions) {
                    Action a = action.OnClone();
                    DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Trying to add action {0} to the current state. Calling action {0}'s OnActionSetup.", a.GetType());
                    a.OnActionSetup(igoap, p.state);                   

                    // Make sure the effects add to the state
                    if (MatchesState(a.effects, p.state)) {
                        
                        // Make sure the action is actually viable
                        if (!a.isViable) {
                            DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Action {0} could not be added because it's not viable.", a.GetType());
                            continue;
                        }

                        DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Action {0} is viable, the state will now be altered with the effects and preconditions.", a.GetType());

                        // Add the actions preconditions
                        List<Condition> state = AddToState(p.state, a.preconditions);
                        DebugText(debug, DebugPlanning.Everything, "Current state after adding action {0}'s preconditions: {1}.", a.GetType(), ConditionsToText(state));
                        // Remove actions effects
                        state = RemoveFromState(state, a.effects);
                        DebugText(debug, DebugPlanning.Everything, "Current state after removing action {0}'s effects: {1}.", a.GetType(), ConditionsToText(state));
                        // Remove preconditions which our current state already fullfills
                        state = RemoveFromState(state, igoap.state);
                        DebugText(debug, DebugPlanning.Everything, "Current state after removing iGoap's state: {0}.", ConditionsToText(state));

                        // Create a new node
                        Node n = new Node(p, a, state);

                        // If the state hasn't changed, the action will be futile
                        if (MatchesState(n.state, p.state)) {
                            DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Action {0} effects did not alter the state. Since this action is futile it will not be added.", a.GetType());
                            continue;
                        }

                        // See if we've reached the solution
                        if (n.state.Count == 0){
                            closedList.Add(n);
                            continue;
                        }

                        DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Action {0} succesfully added to the open list. Current open list count: {1}.", a.GetType(), openList.Count);

                        // Otherwise we add it to the open list
                        openList.Add(n);
                    }
                    else {
                        DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Action {0} is not viable.", a.GetType());
                    }
                }

                // We're done with this node, so we remove it from the list
                openList.Remove(p);

                // Check to see if a solution has been found
                if (openList.Count == 0 && closedList.Count != 0)
                    return NodesToActions(GetCheapestNode(closedList));

                if (openList.Count == 0)
                    break;
            }

            DebugText(debug, DebugPlanning.Basics | DebugPlanning.Everything, "Ran out of viable path options for goal {0}.", goal.GetType());
            return null;
        }

        private static string ConditionsToText(List<Condition> conditions) {
            string s = "";

            if (conditions == null) {
                s += "[Conditions are null! Please fill the condition list]";
                return s;
            }

            foreach (Condition c in conditions)
                s += "[<color=orange> " + c.identifier + " </color>, <color=cyan>" + c.value + " </color>] ";

            return s;
        }

        private static List<Action> NodesToActions(Node n) {
            List<Action> actions = new List<Action>();

            while (true) {
                if (n.action != null) actions.Add(n.action);
                if (n.parent != null) n = n.parent; else break;
            }

            return actions;
        }

        private static Node GetCheapestNode(List<Node> nodes) {
            return nodes.OrderBy(n => n.cost).ToList()[0];
        }

        private static List<Condition> AddToState(List<Condition> current, List<Condition> addOn) {
            List<Condition> next = new List<Condition>(current);

            foreach (Condition c in addOn)
                next.Add(c);

            return next;
        }

        private static List<Condition> RemoveFromState(List<Condition> current, List<Condition> removables) {
            if (removables == null || removables.Count == 0)
                return current;

            List<Condition> next = new List<Condition>(current);

            foreach (Condition r in removables)
                foreach (Condition c in current)
                    if (c.identifier.Equals(r.identifier) && c.value.Equals(r.value))
                        next.Remove(c);

            return next;
        }

        private static bool MatchesState(List<Condition> addOn, List<Condition> current) {
            if (addOn.Count == 0 || current.Count == 0)
                return false;

            foreach (Condition a in addOn) {
                bool match = false;

                foreach (Condition c in current) {
                    if (a.identifier.Equals(c.identifier) && a.value.Equals(c.value)) {
                        match = true;
                        break;
                    }
                }

                if (match == false)
                    return false;
            }

            return true;
        }

        private static void DebugText(DebugPlanning current, DebugPlanning required, string text, params object[] obj) {
            if((current & required) != 0) Debug.Log(string.Format(text, obj));
        }

        private class Node {

            public Node parent;
            public Action action;
            public List<Condition> state;
            public float cost { get { return action.cost + (parent != null && parent.action != null ? parent.action.cost : 0f); } }

            public Node(Node parent, Action action, List<Condition> state) {
                this.parent = parent;
                this.action = action;
                this.state = state;
            }
        }
    }
}
