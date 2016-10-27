using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    public abstract class Action : ConditionHolder {

        #region Public variables
        public IGoap igoap;

        public List<Condition> preconditions = new List<Condition>();
        public List<Condition> effects = new List<Condition>();

        public bool isViable, isFinished, isAborted;
        public virtual float cost { get; set; }

        protected float currentPerformTime, totalPerformTime;
        public float progress { get { return Mathf.Clamp(currentPerformTime / totalPerformTime, 0f, 1f); } }
        #endregion

        #region Public functions
        public Action OnClone() { return (Action)this.MemberwiseClone(); }

        public virtual void OnActionSetup(IGoap igoap, List<Condition> state) { this.igoap = igoap; }
        public virtual void OnActionStart() { }
        public virtual void OnActionPerform() { }
        public virtual void OnActionFinished() { }
        public virtual void OnActionAborted() { }
        #endregion

        #region State Changers
        protected void AddPrecondition(string identifier, object value) { AddPrecondition(new Condition(identifier, value)); }
        protected void AddPrecondition(Condition condition) { AddCondition(condition, preconditions); }

        protected void AddEffect(string identifier, object value) { AddEffect(new Condition(identifier, value)); }
        protected void AddEffect(Condition condition) { AddCondition(condition, effects); }

        protected void UpdatePrecondition(string identifier, object value) { UpdatePrecondition(new Condition(identifier, value)); }
        protected void UpdatePrecondition(Condition condition) { UpdateCondition(condition, preconditions); }

        protected void UpdateEffect(string identifier, object value) { UpdateEffect(new Condition(identifier, value)); }
        protected void UpdateEffect(Condition condition) { UpdateCondition(condition, effects); }

        protected void RemovePrecondition(Condition condition) { RemovePrecondition(condition.identifier); }
        protected void RemovePrecondition(string identifier) { RemoveCondition(identifier, preconditions); }

        protected void RemoveEffect(Condition condition) { RemoveEffect(condition.identifier); }
        protected void RemoveEffect(string identifier) { RemoveCondition(identifier, effects); }
        #endregion
    }
}
