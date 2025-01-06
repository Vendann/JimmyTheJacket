using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource pistol;
    public AudioSource shotgun;
    public AudioSource rifle;
    public AudioSource pistolReload;
    public AudioSource shotgunReload;
    public AudioSource rifleReload;
    public AudioSource grenadeReady;
    public AudioSource itemPickUp;
    public AudioSource explosion;
    public AudioSource step;
    public AudioSource damageReaction;
    public AudioSource music;
    public AudioSource pistolEnemy;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
}
