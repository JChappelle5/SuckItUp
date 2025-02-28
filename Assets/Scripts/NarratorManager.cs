using UnityEngine;

public class NarratorManager : MonoBehaviour
{
    public AudioSource audioSource; // Reference to AudioSource
    public AudioClip[] narratorClips; // Array to store all narration clips

    public void PlayRandomClip()
    {
        if (narratorClips.Length == 0) return; // If no clips are assigned, do nothing

        AudioClip chosenClip = narratorClips[Random.Range(0, narratorClips.Length)];
        audioSource.PlayOneShot(chosenClip);
    }
}
