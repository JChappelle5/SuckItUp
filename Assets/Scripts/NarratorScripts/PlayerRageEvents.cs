using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRageEvents : MonoBehaviour
{
    [Header("Core Configuration")]
    [Tooltip("Conversion rate from Unity units to in-game meters.")]
    public float unityUnitsPerMeter = 5.235f;

    [Header("Repeated Fall Settings")]
    public int repeatFallThreshold = 2;
    public float minFallDistanceForRepeat = 0.3f;

    [Header("Progress / Start Settings")]
    public float significantProgressThreshold = 10f;

    [Header("Stuck Detection")]
    public float stuckTimeThreshold = 30f;

    [Header("New Height Settings")]
    [Tooltip("Minimum meters needed to trigger a 'newHeightEvent' at all.")]
    public float baseHeightCheckpointInterval = 1.5f;
    // Tiered thresholds for awarding frustration relief on new height
    private float smallClimbThreshold = 1.5f;
    private float mediumClimbThreshold = 3f;
    private float bigClimbThreshold = 5f;

    // Frustration: 1..10; start high for beginners (8 gives supportive clips)
    private float frustrationLevel = 8f;
    public float FrustrationLevel => frustrationLevel;

    // Internal fall/landing state
    private bool isFalling = false;
    private bool hasLanded = true;
    private bool significantProgressMade = false;
    private bool returnedToStartThisFall = false;

    private float highestPointBeforeFall;
    private float lastHeightCheckpoint;
    private float initialStartHeight;
    private float highestHeightReached;

    private float velocityThreshold = 0.1f;
    private float lastFallHeight = -9999f;
    private int consecutiveFallCount = 0;
    private float stuckTimer = 0f;

    // Track consecutive successful climbs
    private int consecutiveNewHeightCount = 0;

    // --- NEW: Timer for "No Falls" frustration relief ---
    [Tooltip("How many seconds without falling before first frustration decrease.")]
    public float initialNoFallWait = 30f; // starts at 30 sec
    private float timeWithoutFall = 0f;
    private float currentNoFallThreshold = 0f;

    // Events for Behavior Tree
    private bool bigFallEvent = false;
    private bool repeatedFallEvent = false;
    private bool newHeightEvent = false;

    public bool BigFallEvent => bigFallEvent;
    public bool RepeatedFallEvent => repeatedFallEvent;
    public bool NewHeightEvent => newHeightEvent;

    // --- NEW: Skill test variables ---
    // If the player climbs 3 game meters within the first minute, they are considered skilled.
    private float startTime = 0f;
    private bool skillTestPassed = false;

    void Start()
    {
        highestPointBeforeFall = transform.position.y;
        lastHeightCheckpoint = transform.position.y;
        initialStartHeight = transform.position.y;
        highestHeightReached = initialStartHeight;
        frustrationLevel = 8f;  // Start high for beginners

        // Initialize no-fall timer
        timeWithoutFall = 0f;
        currentNoFallThreshold = initialNoFallWait; // 30 sec

        // Record game start time for the skill test.
        startTime = Time.time;
    }

    void Update()
    {
        float currentHeight = transform.position.y;
        float verticalVelocity = GetComponent<Rigidbody2D>().linearVelocity.y;

        // 1) Detect start of a fall
        if (!isFalling && hasLanded && verticalVelocity < -velocityThreshold)
        {
            isFalling = true;
            hasLanded = false;
            highestPointBeforeFall = currentHeight;
            returnedToStartThisFall = false;
        }

        // 2) Detect landing
        if (isFalling && !hasLanded && Mathf.Abs(verticalVelocity) < velocityThreshold)
        {
            isFalling = false;
            hasLanded = true;

            float fallDistanceMeters = (highestPointBeforeFall - currentHeight) / unityUnitsPerMeter;

            // 2.1) If we made significant progress but fell all the way back — no longer penalize frustration
            if (significantProgressMade && currentHeight <= initialStartHeight + 0.1f)
            {
                significantProgressMade = false;
                highestHeightReached = initialStartHeight;
                returnedToStartThisFall = true;
            }

            // 2.2) Tiered frustration for falls: >3m, >6m, >9m
            bigFallEvent = false; // reset before setting it true
            if (fallDistanceMeters > 9f)
            {
                IncreaseFrustration(3f, "Massive fall");
                bigFallEvent = true;
            }
            else if (fallDistanceMeters > 6f)
            {
                IncreaseFrustration(2f, "Medium fall");
                bigFallEvent = true;
            }
            else if (fallDistanceMeters > 3f)
            {
                IncreaseFrustration(1f, "Small fall");
                bigFallEvent = true;
            }

            // If any fall event occurred, reset the no-fall timer system and break the consecutive climb streak.
            if (bigFallEvent)
            {
                consecutiveNewHeightCount = 0;
                timeWithoutFall = 0f;
                currentNoFallThreshold = initialNoFallWait;
            }

            // 2.3) Repeated falls
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

            // 2.4) Check if we reached a new height using the dynamic threshold!
            float heightCheckpointMeters = (currentHeight - lastHeightCheckpoint) / unityUnitsPerMeter;
            if (heightCheckpointMeters >= GetDynamicNewHeightThreshold())
            {
                newHeightEvent = true;
            }
        }

        // 3) Stuck detection
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

        // 4) Track highest point reached
        if (currentHeight > highestHeightReached)
            highestHeightReached = currentHeight;

        // Significant progress check
        if (!significantProgressMade &&
            (highestHeightReached - initialStartHeight) / unityUnitsPerMeter >= significantProgressThreshold)
        {
            significantProgressMade = true;
            Debug.Log("Significant progress achieved!");
        }

        // Update highestPointBeforeFall if ascending again
        if (currentHeight > highestPointBeforeFall)
            highestPointBeforeFall = currentHeight;

        // 5) Handle "No fall for X seconds" logic,
        // but only if the player is actually moving enough (at least 3 game meters from the last checkpoint)
        float heightDeltaGameMeters = distanceFromLastCheckpoint / unityUnitsPerMeter;
        if (!bigFallEvent && heightDeltaGameMeters >= 3f)
        {
            timeWithoutFall += Time.deltaTime;
            if (timeWithoutFall >= currentNoFallThreshold)
            {
                DecreaseFrustration(1f, $"No falls for {currentNoFallThreshold} seconds");
                timeWithoutFall = 0f;
                currentNoFallThreshold += 10f;
            }
        }

        // 6) Skill test: if within the first minute the player climbs 3 game meters, mark as skilled.
        if (!skillTestPassed && (Time.time - startTime) < 60f)
        {
            float progressFromStart = (highestHeightReached - initialStartHeight) / unityUnitsPerMeter;
            if (progressFromStart >= 3f)
            {
                DecreaseFrustration(7f, "Skilled player: quick progress within 1 minute");
                skillTestPassed = true;
            }
        }
    }

    // -----------------------
    // DYNAMIC THRESHOLDS
    // -----------------------
    // For falls: lower frustration means a smaller threshold so even modest falls trigger feedback.
    // For new heights: We want a mapping such that:
    //   Frustration 1  => 7 m threshold
    //   Frustration 8  => 2.6 m threshold
    private float GetDynamicBigFallThreshold()
    {
        float t = Mathf.InverseLerp(1f, 10f, frustrationLevel);
        // Lerp multiplier from 0.5 (at low frustration) to 1.5 (at high frustration)
        float multiplier = Mathf.Lerp(0.5f, 1.5f, t);
        return 3f * multiplier; // baseBigFallThreshold = 3m
    }

    private float GetDynamicNewHeightThreshold()
    {
        // Clamp frustration to [1, 8] for this mapping.
        float clampedFrustration = Mathf.Clamp(frustrationLevel, 1f, 8f);
        // Normalize: when frustration=1, t=0; when frustration=8, t=1.
        float t = (clampedFrustration - 1f) / 7f;
        // Lerp from 7 m at t=0 to 2.6 m at t=1.
        return Mathf.Lerp(7f, 2.6f, t);
    }

    // -----------------------
    // FRUSTRATION LOGIC
    // -----------------------
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

    // Overload for single-argument calls
    public void DecreaseFrustration(float amount)
    {
        DecreaseFrustration(amount, "Achieved new height or encouragement");
    }

    // -----------------------
    // EVENT RESETTERS (for Behavior Tree)
    // -----------------------
    public void ResetBigFallEvent() => bigFallEvent = false;
    public void ResetRepeatedFallEvent() => repeatedFallEvent = false;
    public void ResetNewHeightEvent() => newHeightEvent = false;

    // -----------------------
    // CHECKPOINT UPDATE
    // -----------------------
    public void UpdateLastHeightCheckpoint()
    {
        float oldCheckpoint = lastHeightCheckpoint;
        lastHeightCheckpoint = transform.position.y;
        ResetNewHeightEvent();
        stuckTimer = 0f;

        float climbFromLastCheckpoint = (lastHeightCheckpoint - oldCheckpoint) / unityUnitsPerMeter;

        // Tiered reward for new height based on the climb distance
        if (climbFromLastCheckpoint > bigClimbThreshold)
        {
            DecreaseFrustration(2f, "Big climb above last checkpoint");
        }
        else if (climbFromLastCheckpoint > mediumClimbThreshold)
        {
            DecreaseFrustration(1f, "Medium climb above last checkpoint");
        }
        else if (climbFromLastCheckpoint > smallClimbThreshold)
        {
            DecreaseFrustration(0.5f, "Small climb above last checkpoint");
        }

        // Reward extra for consecutive successes
        consecutiveNewHeightCount++;
        if (consecutiveNewHeightCount >= 3)
        {
            DecreaseFrustration(1f, "Consecutive new heights in a row!");
            consecutiveNewHeightCount = 0;
        }

        Debug.Log(
            $"Checkpoint updated at {lastHeightCheckpoint} (climbed {climbFromLastCheckpoint:F2}m). " +
            $"Consecutive new heights: {consecutiveNewHeightCount}. Frustration: {frustrationLevel}"
        );
    }
}
