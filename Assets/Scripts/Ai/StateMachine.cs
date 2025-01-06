using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    // public PatrolState patrolState;

    private void Update() {
        if (activeState != null) {
            activeState.Perform();
        }
    }

    public void Initialise() {
        // patrolState = new PatrolState();
        ChangeState(new PatrolState());
    }

    public void ChangeState(BaseState newState) {
        // check activeState != null, run cleanup on activeState
        if (activeState != null) activeState.Exit();

        // change to a new state
        activeState = newState;

        // fail-safe null check to make sure new state wasn't null
        if (activeState != null) {
            // setup new state
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
