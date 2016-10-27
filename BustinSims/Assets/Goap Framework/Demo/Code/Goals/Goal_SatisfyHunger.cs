using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GOAP;

public class Goal_SatisfyHunger : Goal {

    private Entity entity;

    public Goal_SatisfyHunger() {
        // Add the succes conditions
        AddSucces("satisfyHunger", true);

        // Set the priority
        priority = 5;
    }

    public override void OnGoalInitialize(IGoap igoap) {
        // Set the entity for later use
        entity = (Entity)igoap;
    }

    public override bool IsGoalRelevant() {
        // The entity only needs to feed itself if its hungry
        return entity.isHungry;
    }

}
