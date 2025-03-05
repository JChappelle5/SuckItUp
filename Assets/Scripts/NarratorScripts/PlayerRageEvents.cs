using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRageEvents : MonoBehaviour
{
    [Header("References")]
    public NarratorManager narratorManager;
    public HeightTracker heightTracker;

    [Header("Fall & Height Thresholds")]
    public float bigFallThreshold = 6f;
    public float heightCheckpointInterval = 10f;
    public int repeatFallThreshold = 3; // Only play audio after 3 repeated falls in a row
    public float minFallDistanceForRepeat = 3f; // Minimum fall distance to count as "frustrating"

    [Header("Cooldowns")]
    private float narrationCooldown = 0f;
    private float narrationCooldownDuration = 3f;

    private bool isFalling = false;
    private bool hasLanded = true;
    private bool hasFallenOnce = false;
    private int highestPointBeforeFall;
    private int lastHeightCheckpoint;
    private float velocityThreshold = 0.1f;

    // Tracking repeated falls
    private int lastFallHeight = -9999; // Track the last height the player fell from
    private int consecutiveFallCount = 0; // Count consecutive falls from the same height

    void Start()
    {
        lastHeightCheckpoint = Mathf.FloorToInt(heightTracker.playerRb.position.y / 5.45f) + 1;
    }

    void Update()
    {
        if (heightTracker == null) return;

        int currentHeight = Mathf.FloorToInt(heightTracker.playerRb.position.y / 5.45f) + 1;
        float verticalVelocity = heightTracker.playerRb.linearVelocity.y;

        // Detect when the player starts falling
        if (!isFalling && hasLanded && currentHeight < highestPointBeforeFall && verticalVelocity < -velocityThreshold)
        {
            isFalling = true;
            hasLanded = false;
            highestPointBeforeFall = currentHeight;
        }

        // Detect when the player lands
        if (isFalling && !hasLanded && Mathf.Abs(verticalVelocity) < velocityThreshold)
        {
            isFalling = false;
            hasLanded = true;
            int fallDistance = highestPointBeforeFall - currentHeight;

            if (fallDistance >= bigFallThreshold)
            {
                hasFallenOnce = true;

                if (Time.time >= narrationCooldown)
                {
                    if (!narratorManager.isPlayingAudio)
                    {
                        narratorManager.PlayRandomClip(narratorManager.allFalls);
                    }
                    narrationCooldown = Time.time + narrationCooldownDuration;
                }
            }

            // Ignore small falls for repeated fall detection
            if (fallDistance < minFallDistanceForRepeat) return;

            // Check if this is a repeated fall
            if (currentHeight == lastFallHeight)
            {
                consecutiveFallCount++;

                if (consecutiveFallCount >= repeatFallThreshold)
                {
                    if (!narratorManager.isPlayingAudio && narratorManager.repeatedFallClips.Length > 0)
                    {
                        narratorManager.PlayRandomClip(narratorManager.repeatedFallClips);
                    }

                    // Reset counter to avoid immediate spam
                    consecutiveFallCount = 0;
                }
            }
            else
            {
                // Reset counter if the player falls from a different height
                lastFallHeight = currentHeight;
                consecutiveFallCount = 1;
            }
        }

        // Detect when the player surpasses the last height checkpoint by 10 meters
        if (hasFallenOnce && currentHeight >= lastHeightCheckpoint + heightCheckpointInterval)
        {
            lastHeightCheckpoint = currentHeight;

            if (!narratorManager.isPlayingAudio && narratorManager.newHeightClips.Length > 0)
            {
                narratorManager.PlayRandomClip(narratorManager.newHeightClips);
            }
        }

        // Update highest point if player reaches a new max height
        if (currentHeight > highestPointBeforeFall)
        {
            highestPointBeforeFall = currentHeight;
        }
    }
}
