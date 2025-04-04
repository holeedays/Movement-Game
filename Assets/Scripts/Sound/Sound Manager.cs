using Unity.VisualScripting;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    [Header("References")]

    public AudioSource audioSource;
    public AudioClip jumpSound;

    public void PlayJumpSound()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
    }


}
