using UnityEngine;
using System.Collections;
using GOAP;

public class Goal_CollectFood : Goal {

    private Entity entity;

    public Goal_CollectFood() {
        // Add the succes conditions
        AddSucces("collectFood", true);

        // Set the priority
        priority = 1;
    }

    public override void OnGoalInitialize(IGoap igoap) {
        // Set the entity for later use
        entity = (Entity)igoap;
    }

    public override bool IsGoalRelevant() {
        // We can only collect food if we know about at least one food spot
        return entity.knownBushes.Count != 0 || entity.knownCarcasses.Count != 0;
    }

}
