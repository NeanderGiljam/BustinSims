using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GOAP;

public class Entity : MonoBehaviour, IGoap {

    // These variables are essential for the GoapUpdater
    public List<Condition> state { get; set; }
    public List<Goal> availableGoals { get; set; }
    public List<Action> availableActions { get; set; }

    // The entity needs to know about the available food posts
    public List<Transform> knownBushes;
    public List<Transform> knownCarcasses;

    // Returns the closests transform based on the distance
    public Transform closestsBush { get { return (knownBushes == null || knownBushes.Count == 0 ? null : knownBushes.OrderBy(d => Vector2.Distance(transform.position, d.position)).ToList()[0]); } }
    public Transform closestsCarcass { get { return (knownCarcasses == null || knownCarcasses.Count == 0 ? null : knownCarcasses.OrderBy(d => Vector2.Distance(transform.position, d.position)).ToList()[0]); } }

    public int amountOfFood = 0;
    public float speed = 4f;
    public bool hasKnifeOnStart = true;
    public bool isHungry = true;    

    private float hungerTimer;
    
    private void Awake() {
        // Create the correct entity state
        state = new List<Condition>();
        state.Add(new Condition("hasKnife", hasKnifeOnStart));

        // Give the entity the correct actions and goals
        availableGoals = new List<Goal>() { ScriptableObject.CreateInstance<Goal_CollectFood>(), ScriptableObject.CreateInstance<Goal_SatisfyHunger>(), };
        availableActions = new List<Action>() { ScriptableObject.CreateInstance<Action_MoveTo>(), ScriptableObject.CreateInstance<Action_EatFood>(), ScriptableObject.CreateInstance<Action_CollectFoodFromBush>(), ScriptableObject.CreateInstance<Action_CollectFoodFromCarcass>() };

        // Initialize the goals
        foreach (Goal g in availableGoals) g.OnGoalInitialize(this);
    }

    private void Update() {
        // A simple timer which runs as soon as the entity is not hungry. If the timer passes 4 seconds the entity's state is set to hungry
        if (isHungry) return;

        hungerTimer += Time.deltaTime;

        if (hungerTimer >= 4f) {
            hungerTimer = 0f;
            isHungry = true;
        }
    }
    
}
