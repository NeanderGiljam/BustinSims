using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    public abstract class Goal : ConditionHolder {

        #region Public variables
        public IGoap igoap;

        public List<Condition> succes = new List<Condition>();

        public virtual float priority { get; set; }
        #endregion

        #region Public functions
        public virtual void OnGoalInitialize(IGoap igoap) { this.igoap = igoap; }
        public virtual void OnGoalSetup() { }
        public virtual void OnGoalFinished() { }
        public virtual void OnGoalAborted() { }

        public abstract bool IsGoalRelevant();
        #endregion

        #region State changers
        protected void AddSucces(string identifier, object value) { AddSucces(new Condition(identifier, value)); }
        protected void AddSucces(Condition condition) { AddCondition(condition, succes); }

        protected void UpdateSucces(string identifier, object value) { UpdateSucces(new Condition(identifier, value)); }
        protected void UpdateSucces(Condition condition) { UpdateCondition(condition, succes); }

        protected void RemoveSucces(Condition condition) { RemoveSucces(condition.identifier); }
        protected void RemoveSucces(string identifier) { RemoveCondition(identifier, succes); }
        #endregion
    }
}
