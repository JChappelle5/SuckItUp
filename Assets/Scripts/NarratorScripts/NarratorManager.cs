using UnityEngine;

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

    public bool isPlayingAudio => audioSource.isPlaying;

    public void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Select clip based on frustration level
    public void PlayClipBasedOnFrustration(AudioClip[] low, AudioClip[] medium, AudioClip[] high, float frustration)
    {
        AudioClip[] selectedClips;

        if (frustration <= 3)
            selectedClips = low;
        else if (frustration <= 7)
            selectedClips = medium;
        else
            selectedClips = high;

        PlayRandomClip(selectedClips);
    }
}
