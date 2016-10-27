using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    [RequireComponent(typeof(IGoap))]
    public class GoapUpdater : MonoBehaviour {
        
        public List<Action> plan { get; set; }
        public Goal currentGoal { get; set; }
        public Action currentAction { get; set; }

        public System.Action<Goal> OnGoalFound, OnGoalFinished, OnGoalAborted;
        public System.Action<Action> OnActionStarted, OnActionFinished, OnActionAborted;
        public float searchingTime = .2f;
        public bool automaticallySearchForNextPlan = true;
        public Planner.DebugPlanning debug = Planner.DebugPlanning.None;

        private enum State { Idle, Selection, Perform };
        private State state = State.Idle;

        private IGoap igoap;
        private float searchingTimer;

        private void Awake() {
            igoap = GetComponent<IGoap>();
        }

        private void Update() {
            if (igoap == null)
                return;

            switch (state) {
                default:
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Selection:
                    UpdateSelection();
                    break;
                case State.Perform:
                    UpdatePerform();
                    break;
            }
        }

        #region Public functions
        /// <summary>
        /// Will try to formulate a plan for the specific goal
        /// </summary>
        /// <param name="goal">Goal the updates will try to push</param>
        public void PushGoal(Goal goal) {
            AbortGoal();
            UpdateIdle(goal);
        }

        /// <summary>
        /// Aborts the current goal, after which the updater will start looking for the new best goal
        /// </summary>
        public void AbortGoal() {
            InvokeAction(OnGoalAborted, currentGoal);

            if (state != State.Idle)
                currentGoal.OnGoalAborted();

            if (state == State.Perform) {
                InvokeAction(OnActionAborted, currentAction);
                currentAction.OnActionAborted();
            }

            state = State.Idle;
        }
        #endregion

        #region Actions
        private void InvokeAction(System.Action<Goal> action, Goal value) {
            if (action != null) action.Invoke(value);
        }

        private void InvokeAction(System.Action<Action> action, Action value) {
            if (action != null) action.Invoke(value);
        }
        #endregion

        #region States
        private void UpdateIdle() {
            searchingTimer += Time.deltaTime;

            if (!automaticallySearchForNextPlan || searchingTimer < searchingTime)
                return;

            // Tries to find a plan based on all available goals
            Goal goal;
            UpdateIdle(Planner.FormulatePlan(igoap, igoap.availableActions, igoap.availableGoals, out goal, debug), goal);
        }

        private void UpdateIdle(Goal goal) {
            // Tries to find a plan based on the specific goal
            UpdateIdle(Planner.FormulatePlan(igoap, igoap.availableActions, goal, out goal, debug), goal);
        }

        private void UpdateIdle(List<Action> plan, Goal goal) {
            // Reset the plan searching timer
            searchingTimer = 0f;

            if (plan != null) {
                // Set current goal & plan and switch to selection state
                currentGoal = goal;
                this.plan = plan;

                InvokeAction(OnGoalFound, currentGoal);

                state = State.Selection;
            }
        }

        private void UpdateSelection() {
            // Either the plan is null or all actions are performed
            if (plan == null || plan.Count == 0) {
                InvokeAction(OnGoalFinished, currentGoal);

                currentGoal.OnGoalFinished();
                state = State.Idle;
            }
            else {
                // Set the new current action and switches to the perform state
                currentAction = plan[0];
                currentAction.OnActionStart();

                InvokeAction(OnActionStarted, currentAction);
                state = State.Perform;
            }
        }

        private void UpdatePerform() {
            // Perform the current action
            currentAction.OnActionPerform();

            // Checks if the current action has been aborted or finished, and updates accordingly
            if (currentAction.isAborted) {
                AbortGoal();
            }
            else if (currentAction.isFinished) {
                InvokeAction(OnActionFinished, currentAction);

                currentAction.OnActionFinished();
                plan.RemoveAt(0);

                state = State.Selection;
            }

        }
        #endregion
    }
}
