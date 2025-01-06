using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;

    void Start()
    {
        Destroy(gameObject, _timeToDestroy);
    }
}
