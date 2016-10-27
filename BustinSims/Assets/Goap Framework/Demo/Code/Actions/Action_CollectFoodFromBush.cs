using UnityEngine;
using System.Collections;
using GOAP;

public class Action_CollectFoodFromBush : Action {

    private Entity entity;
    private Transform target;

    private int ammountOfFoodCollected = 1;

    public Action_CollectFoodFromBush() {
        // Add preconditions
        AddPrecondition("inRangeOf", null);

        // Add effects
        AddEffect("collectFood", true);

        // Set the cost and the time if takes to 'collect' the food from the bush
        cost = 2;
        totalPerformTime = 2f;
    }

    public override void OnActionSetup(IGoap igoap, System.Collections.Generic.List<Condition> state) {
        entity = (Entity)igoap;

        // Get the closests bush the entity knows off, and if it's not null the action is viable
        target = entity.closestsBush;
        UpdatePrecondition("inRangeOf", target);

        isViable = target != null;
    }

    public override void OnActionPerform() {
        // Simply add to the timer
        currentPerformTime += Time.deltaTime;

        // currentPerformTime / totalPerformTime = progress, so we can use progress to quickly check
        if (progress == 1f)
            isFinished = true;
    }

    public override void OnActionFinished() {
        Debug.Log(string.Format("Collected <color=orange>{0} piece(s)</color> of food from <color=cyan>{1}</color>.", ammountOfFoodCollected, target.name));

        // Since the action is done, we add food, remove the bush from the entity's known list and them remove the carcass from the scene
        entity.amountOfFood += ammountOfFoodCollected;
        entity.knownBushes.Remove(target);
        Destroy(target.gameObject);
    }

}
