using UnityEngine;

public class NarratorManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Mild & Major Falls")]
    public AudioClip[] mildFallClips;
    public AudioClip[] majorFallClips;

    [Header("Repeated Falls")]
    public AudioClip[] repeatedFallClips;

    [Header("Climbing")]
    public AudioClip[] climbingClips;

    [Header("New Heights")]
    public AudioClip[] newHeightClips;

    public void PlayRandomClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0) return;
        AudioClip chosenClip = clipArray[Random.Range(0, clipArray.Length)];
        audioSource.PlayOneShot(chosenClip);
    }
}
