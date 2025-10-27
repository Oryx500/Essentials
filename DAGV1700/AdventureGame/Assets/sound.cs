using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    public AudioSource soundEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // or whatever triggers it
        {
            soundEffect.Play();
        }
    }
}
