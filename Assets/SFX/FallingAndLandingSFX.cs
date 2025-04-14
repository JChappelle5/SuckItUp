using UnityEngine;

public class FallingAndLandingSFX : MonoBehaviour
{
    [Header("Player References")]
    public Rigidbody2D playerRb;
    public PlayerController playerController;
    // Or whichever script has isGrounded if you want an "isGrounded" check.

    [Header("Audio Source for Falling Loop")]
    public AudioSource fallingAudioSource;
    // Typically set this AudioSource to Loop=true in the Inspector for a continuous whoosh.

    [Header("Clips")]
    public AudioClip fallingClip;    // The wind/breeze loop
    public AudioClip landingClip;    // The thud or impact SFX

    [Header("Settings")]
    [Tooltip("We consider the player to be 'truly falling' if velocity.y < this threshold and not grounded.")]
    public float fallingVelocityThreshold = -3f;

    private bool isFalling = false; // Tracks if we're currently playing the falling sound

    void Update()
    {
        // 1) Check if grounded
        bool grounded = (playerController != null) ? playerController.isGrounded : false;

        // 2) Read vertical velocity
        float vY = (playerRb != null) ? playerRb.linearVelocity.y : 0f;

        // 3) Should we be playing the falling sound right now?
        bool shouldFall = !grounded && (vY < fallingVelocityThreshold);

        // If we SHOULD fall but aren't yet
        if (shouldFall && !isFalling)
        {
            // Start the falling whoosh
            if (fallingAudioSource != null && fallingClip != null)
            {
                // Make sure the AudioSource is set to Loop=true for a continuous whoosh
                fallingAudioSource.clip = fallingClip;
                fallingAudioSource.loop = true;
                fallingAudioSource.Play();
                isFalling = true;
            }
        }
        // If we were falling, but now we landed or velocity is above threshold
        else if (!shouldFall && isFalling)
        {
            // Stop the falling loop
            if (fallingAudioSource != null && fallingAudioSource.isPlaying)
            {
                fallingAudioSource.Stop();
            }
            isFalling = false;

            // Immediately play the landing sound
            if (landingClip != null && fallingAudioSource != null)
            {
                // We'll use PlayOneShot so it doesn't require the clip to be on the AudioSource
                fallingAudioSource.PlayOneShot(landingClip);
            }
        }
    }
}
