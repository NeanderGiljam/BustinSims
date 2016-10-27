using UnityEngine;
using System.Collections;
using GOAP;

public class Action_CollectFoodFromCarcass : Action {

    private Entity entity;
    private Transform target;

    private int ammountOfFoodCollected = 2;

    public Action_CollectFoodFromCarcass() {
        // Add preconditions
        AddPrecondition("inRangeOf", null);
        AddPrecondition("hasKnife", true);

        // Add effects
        AddEffect("collectFood", true);

        // Set the cost and the time if takes to 'collect' the food from the carcass
        cost = 1;
        totalPerformTime = 2f;
    }

    public override void OnActionSetup(IGoap igoap, System.Collections.Generic.List<Condition> state) {
        entity = (Entity)igoap;

        // Get the closests carcass the entity knows off, and if it's not null the action is viable
        target = entity.closestsCarcass;
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

        // Since the action is done, we add food, remove the carcass from the entity's known list and them remove the carcass from the scene
        entity.amountOfFood += ammountOfFoodCollected;
        entity.knownCarcasses.Remove(target);
        Destroy(target.gameObject);
    }
    
}
