using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRageEvents : MonoBehaviour
{
    [Header("References")]
    public NarratorManager narratorManager;
    public HeightTracker heightTracker;

    [Header("Unit Conversion")]
    [SerializeField] private float unityUnitsPerMeter = 5.235f;
    public float UnityUnitsPerMeter => unityUnitsPerMeter;

    [Header("Thresholds (Game Meters)")]
    public float bigFallThreshold = 0.7f;
    public float heightCheckpointInterval = 0.5f;
    public int repeatFallThreshold = 2;
    public float minFallDistanceForRepeat = 0.3f;
    public float significantProgressThreshold = 10f;  // 10 meters for "back to start" penalty

    [Header("Stuck Timer Settings")]
    public float stuckTimeThreshold = 30f;

    private bool isFalling = false;
    private bool hasLanded = true;
    private float highestPointBeforeFall;
    private float lastHeightCheckpoint;
    private float velocityThreshold = 0.1f;

    private float lastFallHeight = -9999f;
    private int consecutiveFallCount = 0;

    private float frustrationLevel = 6f;

    private bool bigFallEvent = false;
    private bool repeatedFallEvent = false;
    private bool newHeightEvent = false;

    private float stuckTimer = 0f;

    private float initialStartHeight;
    private bool significantProgressMade = false;
    private float highestHeightReached;

    private bool returnedToStartThisFall = false;

    public bool BigFallEvent => bigFallEvent;
    public bool RepeatedFallEvent => repeatedFallEvent;
    public bool NewHeightEvent => newHeightEvent;

    public void ResetBigFallEvent() => bigFallEvent = false;
    public void ResetRepeatedFallEvent() => repeatedFallEvent = false;
    public void ResetNewHeightEvent() => newHeightEvent = false;

    void Start()
    {
        highestPointBeforeFall = heightTracker.playerRb.position.y;
        lastHeightCheckpoint = heightTracker.playerRb.position.y;
        initialStartHeight = heightTracker.playerRb.position.y;
        highestHeightReached = initialStartHeight;
        frustrationLevel = 6f;
    }

    void Update()
    {
        float currentHeight = heightTracker.playerRb.position.y;
        float verticalVelocity = heightTracker.playerRb.linearVelocity.y;

        if (!isFalling && hasLanded && verticalVelocity < -velocityThreshold)
        {
            isFalling = true;
            hasLanded = false;
            highestPointBeforeFall = currentHeight;
            returnedToStartThisFall = false;  // Reset clearly at new fall
        }

        if (isFalling && !hasLanded && Mathf.Abs(verticalVelocity) < velocityThreshold)
        {
            isFalling = false;
            hasLanded = true;

            float fallDistanceMeters = (highestPointBeforeFall - currentHeight) / unityUnitsPerMeter;

            // Check falling back to beginning clearly first
            if (significantProgressMade && currentHeight <= initialStartHeight + 0.1f)
            {
                IncreaseFrustration(2f, "Fell all the way back to the beginning after significant progress");
                significantProgressMade = false;
                highestHeightReached = initialStartHeight;
                returnedToStartThisFall = true;
            }

            // Only trigger big fall if didn't just return to start
            if (!returnedToStartThisFall && fallDistanceMeters >= bigFallThreshold)
            {
                IncreaseFrustration(1f, "Big fall detected");
                bigFallEvent = true;
            }

            if (fallDistanceMeters >= minFallDistanceForRepeat)
            {
                if (Mathf.Abs(currentHeight - lastFallHeight) < 0.1f)
                {
                    consecutiveFallCount++;
                    if (consecutiveFallCount >= repeatFallThreshold)
                    {
                        IncreaseFrustration(2f, "Repeated falls at the same height");
                        repeatedFallEvent = true;
                    }
                }
                else
                {
                    lastFallHeight = currentHeight;
                    consecutiveFallCount = 1;
                }
            }

            float heightCheckpointMeters = (currentHeight - lastHeightCheckpoint) / unityUnitsPerMeter;
            if (heightCheckpointMeters >= heightCheckpointInterval)
            {
                newHeightEvent = true;
            }
        }

        // Stuck detection logic
        float distanceFromLastCheckpoint = Mathf.Abs(currentHeight - lastHeightCheckpoint);
        if (distanceFromLastCheckpoint < 0.1f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTimeThreshold)
            {
                IncreaseFrustration(1f, "Stuck at the same height for too long");
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        // Track highest height reached clearly
        if (currentHeight > highestHeightReached)
            highestHeightReached = currentHeight;

        if (!significantProgressMade &&
            (highestHeightReached - initialStartHeight) / unityUnitsPerMeter >= significantProgressThreshold)
        {
            significantProgressMade = true;
            Debug.Log("Significant progress achieved!");
        }

        if (currentHeight > highestPointBeforeFall)
            highestPointBeforeFall = currentHeight;
    }

    public bool IsLanded => hasLanded;

    public void UpdateLastHeightCheckpoint()
    {
        lastHeightCheckpoint = CurrentHeight;
        ResetNewHeightEvent();
        stuckTimer = 0f;

        // Decrease frustration on achieving a new height
        DecreaseFrustration(1f, "Achieved a new height");
        Debug.Log($"Checkpoint updated at height: {lastHeightCheckpoint}");
    }

    public void IncreaseFrustration(float amount, string reason)
    {
        frustrationLevel = Mathf.Clamp(frustrationLevel + amount, 1f, 10f);
        Debug.Log($"Frustration increased by {amount}. Reason: {reason}. Current Level: {frustrationLevel}");
    }

    public void DecreaseFrustration(float amount, string reason)
    {
        frustrationLevel = Mathf.Clamp(frustrationLevel - amount, 1f, 10f);
        Debug.Log($"Frustration decreased by {amount}. Reason: {reason}. Current Level: {frustrationLevel}");
    }

    public void DecreaseFrustration(float amount)
    {
        DecreaseFrustration(amount, "Achieved new height or narrator provided encouragement");
    }

    public float HighestPointBeforeFall => highestPointBeforeFall;
    public float LastHeightCheckpoint => lastHeightCheckpoint;
    public int ConsecutiveFallCount => consecutiveFallCount;
    public float FrustrationLevel => frustrationLevel;
    public float CurrentHeight => heightTracker.playerRb.position.y;
}
