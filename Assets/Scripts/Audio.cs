using UnityEngine;

public class Audio : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip clip;

    void Start() {
        if (!audioSource.isPlaying) audioSource.PlayOneShot(clip);
    }
}