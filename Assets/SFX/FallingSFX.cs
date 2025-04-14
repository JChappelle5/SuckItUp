using UnityEngine;

public class FallingSFX : MonoBehaviour
{
    [Header("Player References")]
    [Tooltip("The player's Rigidbody2D, so we can read its vertical velocity.")]
    public Rigidbody2D playerRb;

    [Tooltip("If your script has an isGrounded bool, drag the player script here. Otherwise, you can check velocity alone.")]
    public PlayerController playerController;
    // Or whichever script has isGrounded = true/false.

    [Header("Audio")]
    [Tooltip("AudioSource to play the falling sound.")]
    public AudioSource fallingAudioSource;

    [Tooltip("The single, long whoosh/breeze clip.")]
    public AudioClip fallingClip;

    [Header("Settings")]
    [Tooltip("We consider the player falling if vertical velocity < this threshold, e.g. -1f, -3f, etc.")]
    public float fallingVelocityThreshold = -2f;

    private bool isCurrentlyPlaying = false;

    void Update()
    {
        // 1) Check if the player is grounded (if you have that variable).
        bool grounded = (playerController != null) ? playerController.isGrounded : false;

        // 2) Read the player's vertical velocity from the Rigidbody.
        float verticalVelocity = (playerRb != null) ? playerRb.linearVelocity.y : 0f;

        // 3) Decide if the player is truly 'falling' (not grounded, velocity < threshold).
        bool shouldPlay = !grounded && (verticalVelocity < fallingVelocityThreshold);

        // If we SHOULD play, but the audio isn't playing yet, start it.
        if (shouldPlay && !isCurrentlyPlaying)
        {
            if (fallingAudioSource != null && fallingClip != null)
            {
                fallingAudioSource.clip = fallingClip;  // Assign the clip each time if needed.
                fallingAudioSource.Play();
                isCurrentlyPlaying = true;
            }
        }
        // If we should NOT be playing, but the clip is still going, stop it.
        else if (!shouldPlay && isCurrentlyPlaying)
        {
            if (fallingAudioSource != null && fallingAudioSource.isPlaying)
            {
                fallingAudioSource.Stop();
            }
            isCurrentlyPlaying = false;
        }
    }
}
