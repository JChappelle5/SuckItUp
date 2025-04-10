using UnityEngine;
using System.Collections.Generic;

public class NarratorManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Big Fall Clips")]
    public AudioClip[] bigFallLowFrustration;
    public AudioClip[] bigFallMediumFrustration;
    public AudioClip[] bigFallHighFrustration;

    [Header("Repeated Fall Clips")]
    public AudioClip[] repeatedFallLowFrustration;
    public AudioClip[] repeatedFallMediumFrustration;
    public AudioClip[] repeatedFallHighFrustration;

    [Header("New Height Clips")]
    public AudioClip[] newHeightLowFrustration;
    public AudioClip[] newHeightMediumFrustration;
    public AudioClip[] newHeightHighFrustration;

    [Header("Intro Clips")]
    public AudioClip[] introClips;

    // Track previously played clips to avoid repetition
    private Dictionary<AudioClip[], List<AudioClip>> clipHistory = new Dictionary<AudioClip[], List<AudioClip>>();

    public bool isPlayingAudio => audioSource.isPlaying;

    public void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        // Initialize clip history if not yet done
        if (!clipHistory.ContainsKey(clips) || clipHistory[clips].Count == 0)
        {
            clipHistory[clips] = new List<AudioClip>(clips);
        }

        // Select a clip randomly from available history
        List<AudioClip> availableClips = clipHistory[clips];
        int randomIndex = Random.Range(0, availableClips.Count);
        AudioClip clip = availableClips[randomIndex];

        // Play selected clip
        audioSource.clip = clip;
        audioSource.Play();

        // Remove clip from history to prevent immediate repetition
        availableClips.RemoveAt(randomIndex);
    }

    public void PlayClipBasedOnFrustration(AudioClip[] low, AudioClip[] medium, AudioClip[] high, float frustration)
    {
        if (frustration <= 3)
            PlayRandomClip(low);
        else if (frustration <= 8)
            PlayRandomClip(medium);
        else
            PlayRandomClip(high);
    }

    public void PlayIntroClip()
    {
        PlayRandomClip(introClips);
    }

}
