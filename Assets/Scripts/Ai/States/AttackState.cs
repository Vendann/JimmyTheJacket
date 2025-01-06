using UnityEngine;

public class AttackState : BaseState {
    private float _moveTimer;
    private float _losePlayerTimer;
    private float _shotTimer;

    public override void Enter() {
    }

    public override void Exit() {
    }

    public override void Perform() {
        if (enemy.CanSeePlayer()) {
            _losePlayerTimer = 0;
            _moveTimer += Time.deltaTime;
            _shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);

            if (_shotTimer > enemy.fireRate) {
                Shoot();
            }

            if (_moveTimer > Random.Range(0.5f, 2f)) {
                if (enemy.inActionBehaviour == InActionBehaviour.Mixed) enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5f));
                else if (enemy.inActionBehaviour == InActionBehaviour.Agressive) enemy.Agent.SetDestination(enemy.Player.transform.position);
                else enemy.Agent.SetDestination(- enemy.transform.forward);
                _moveTimer = 0;
            }

            enemy.LastKnownPos = enemy.Player.transform.position;
        }

        else {
            _losePlayerTimer += Time.deltaTime;
            if (_losePlayerTimer > 8f) {
                // Change to the search state
                // if (enemy.inActionBehaviour == InActionBehaviour.Mixed || enemy.inActionBehaviour == InActionBehaviour.Agressive)
                    //stateMachine.ChangeState(new SearchState());
                // else
                stateMachine.ChangeState(new PatrolState());
            } 
        }
    }

    public void Shoot() {

        float x, y;

        x = Random.Range(-enemy.spread, enemy.spread);
        y = Random.Range(-enemy.spread, enemy.spread);

        Vector2 dir = new Vector2(x, y);
        dir.Normalize();

        Vector3 shotDirection = enemy.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(enemy.transform.position, shotDirection, out RaycastHit _hit, enemy.sightDistance)) {
            var damageable = _hit.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.ReceiveDamage(enemy.damage, _hit.point);
        }

        _shotTimer = 0;
        
        enemy.muzzleFlash.Play();

        SoundManager.Instance.pistolEnemy = enemy.pistolEnemy;
        SoundManager.Instance.pistolEnemy.Play();
    }
}
