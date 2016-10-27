using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GOAP;

public class Action_EatFood : Action {

    private Entity entity;
    private float eatingTime = 1f;

    public Action_EatFood() {
        AddEffect("satisfyHunger", true);

        totalPerformTime = eatingTime;
    }

    public override void OnActionSetup(IGoap igoap, List<Condition> state) {
        entity = (Entity)igoap;

        // Only set the action to viable if the entity has any food
        isViable = entity.amountOfFood > 0;
    }
    
    public override void OnActionPerform() {
        // Simply add to the timer
        currentPerformTime += Time.deltaTime;

        // currentPerformTime / totalPerformTime = progress, so we can use progress to quickly check
        if (progress == 1f)
            isFinished = true;
    }

    public override void OnActionFinished() {
        // Since the action is done, we remove a piece of food and set the entity to not hungry anymore
        entity.amountOfFood--;
        entity.isHungry = false;

        Debug.Log(string.Format("Ate a piece of food! <color=orange>{0} piece(s)</color> of food left. Hunger satisfied is <color=cyan>{1}</color>.", entity.amountOfFood, !entity.isHungry));
    }

}
