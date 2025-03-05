using UnityEngine;
using System.Collections;

public class NarratorManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("All Falls (Used for big falls)")]
    public AudioClip[] allFalls;

    [Header("New Heights")]
    public AudioClip[] newHeightClips;

    [Header("Repeated Falls")]
    public AudioClip[] repeatedFallClips; // Added this for repeated fall audios

    public bool isPlayingAudio = false; // Tracks whether an audio clip is currently playing

    public void PlayRandomClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0 || isPlayingAudio) return;

        AudioClip chosenClip = clipArray[Random.Range(0, clipArray.Length)];
        audioSource.PlayOneShot(chosenClip);
        isPlayingAudio = true;

        StartCoroutine(ResetAudioLock(chosenClip.length));
    }

    private IEnumerator ResetAudioLock(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPlayingAudio = false;
    }
}
