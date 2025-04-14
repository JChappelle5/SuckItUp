using UnityEngine;

public class PlungerSFX : MonoBehaviour
{
    [Header("References")]
    public PlungerMovement flingScript;         // Correct class name!
    public Rigidbody2D playerRb;                // Player's Rigidbody2D
    public AudioSource sfxSource;               // Audio source to play SFX
    public AudioClip plungerLaunchClip;         // Your launch sound effect

    [Header("Launch Detection")]
    public float launchVelocityThreshold = 5f;

    private bool wasCharging = false;

    void Update()
    {
        if (flingScript == null || playerRb == null) return;

        bool isCharging = PlungerMovement.isCharging;

        if (wasCharging && !isCharging && playerRb.linearVelocity.magnitude > launchVelocityThreshold)
        {
            PlayLaunchSFX();
        }

        wasCharging = isCharging;
    }

    void PlayLaunchSFX()
    {
        if (sfxSource != null && plungerLaunchClip != null)
        {
            sfxSource.PlayOneShot(plungerLaunchClip);
        }
    }
}
