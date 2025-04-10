using UnityEngine;

public class NarratorIntroTrigger : MonoBehaviour
{
    // Reference to your NarratorManager; assign it in the Inspector.
    public NarratorManager narratorManager;

    // This flag makes sure the intro is played only once.
    private bool hasPlayedIntro = false;

    void Start()
    {
        // Optionally add a delay before playing the intro (e.g., 1.5 seconds)
        Invoke(nameof(PlayIntroIfNeeded), 1.5f);
    }

    void PlayIntroIfNeeded()
    {
        if (!hasPlayedIntro && narratorManager != null)
        {
            narratorManager.PlayIntroClip();
            hasPlayedIntro = true;
            Debug.Log("Intro clip played.");
        }
    }
}