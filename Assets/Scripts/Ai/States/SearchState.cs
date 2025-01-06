using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState {
    private float _searchTimer;
    private float _moveTimer;

    public override void Enter() {
        enemy.Agent.SetDestination(enemy.LastKnownPos);
    }

    public override void Perform() {
        if (enemy.CanSeePlayer()) stateMachine.ChangeState(new AttackState());
        
        if (enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance) {
            _searchTimer += Time.deltaTime;
            _moveTimer += Time.deltaTime;

            if (_moveTimer > Random.Range(3f, 5f)) {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 10f));
                _moveTimer = 0;
            }

            if (_searchTimer > Random.Range(6f, 8f)) {
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public override void Exit() {
    }
}
