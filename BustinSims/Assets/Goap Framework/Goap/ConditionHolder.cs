using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    public abstract class ConditionHolder : ScriptableObject {

        protected void AddCondition(string identifier, object value, List<Condition> conditions) { AddCondition(new Condition(identifier, value), conditions); }
        protected void AddCondition(Condition condition, List<Condition> conditions) {
            conditions.Add(condition);
        }

        protected void UpdateCondition(string identifier, object value, List<Condition> conditions) { UpdateCondition(identifier, value, conditions); }
        protected void UpdateCondition(Condition condition, List<Condition> conditions) {
            foreach (Condition con in conditions)
                if (con.identifier == condition.identifier) {
                    con.value = condition.value;
                    break;
                }
        }

        protected void RemoveCondition(string identifier, List<Condition> conditions) {
            Condition c = null;

            foreach (Condition con in conditions)
                if (con.identifier == identifier) {
                    c = con;
                    break;
                }

            if (c != null) conditions.Remove(c);
        }

    }
}
