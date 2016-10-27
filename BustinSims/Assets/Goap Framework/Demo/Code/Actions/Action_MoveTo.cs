using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GOAP;

public class Action_MoveTo : Action {

    private Entity entity;
    private Transform target;

    private float distanceUntilAtTarget = .4f;

    public Action_MoveTo() {
        AddEffect("inRangeOf", null);
    }

    public override void OnActionSetup(IGoap igoap, List<Condition> state) {
        entity = (Entity)igoap;

        // Loop through the conditions state, and see if we can find a condition called "inRangeOf"
        foreach (Condition c in state) {
            // If we find one, we use the value to set our target
            if (c.identifier == "inRangeOf") {
                target = (Transform)c.value;
                break;
            }
        }

        // Update the effect with the target, and make sure the action is viable based on the target
        UpdateEffect("inRangeOf", target);
        isViable = target != null;
    }

    public override void OnActionPerform() {
        // If the target has dissapeared, we want to abort the action since we don't have a target anymore
        if (target == null) {
            isAborted = true;
            return;
        }

        // Move the entity towards the target using its speed
        entity.transform.position = Vector2.MoveTowards(entity.transform.position, target.position, entity.speed * Time.deltaTime);

        // If we're close enough the action is finished
        if (Vector2.Distance(entity.transform.position, target.position) <= distanceUntilAtTarget)
            isFinished = true;
    }
}
