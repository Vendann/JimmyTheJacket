using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine _stateMachine;
    private NavMeshAgent _agent;
    private GameObject _player;
    private Vector3 _lastKnownPos;

    public NavMeshAgent Agent => _agent;
    public GameObject Player => _player;
    public Vector3 LastKnownPos { get => _lastKnownPos; set => _lastKnownPos = value; }

    public AudioSource pistolEnemy;
    public AudioSource damageReaction;
    public Path path;
    public InActionBehaviour inActionBehaviour;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;

    [Header("Weapon Values")]
    // public Transform gunBarrel;
    public ParticleSystem muzzleFlash;
    public float fireRate;
    public float damage;
    public float spread;

    [SerializeField] private string _currentState;
    
    private void Start() {
        _stateMachine = GetComponent<StateMachine>();
        _agent = GetComponent<NavMeshAgent>();
        _stateMachine.Initialise();

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        CanSeePlayer();
        _currentState = _stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer() {
        if (_player != null) {
            // is the player close enough to be seen
            
            if (Vector3.Distance(transform.position, _player.transform.position) < sightDistance) {
                Vector3 targetDirection = _player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                
                if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView) {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();

                    if (Physics.Raycast(ray, out hitInfo, sightDistance)) {
                        if (hitInfo.transform.gameObject == _player) {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}

public enum InActionBehaviour {
    Agressive, Passive, Mixed
}