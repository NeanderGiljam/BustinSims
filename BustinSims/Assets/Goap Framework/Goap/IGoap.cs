using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOAP {
    public interface IGoap {

        List<Condition> state { get; set; }
        List<Goal> availableGoals { get; set; }
        List<Action> availableActions { get; set; }

    }
}
