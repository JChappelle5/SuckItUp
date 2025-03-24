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

    private bool isFalling = false;
    private bool hasLanded = true;
    private float highestPointBeforeFall;
    private float lastHeightCheckpoint;
    private float velocityThreshold = 0.1f;

    private float lastFallHeight = -9999f;
    private int consecutiveFallCount = 0;

    private float frustrationLevel = 0f;

    void Start()
    {
        highestPointBeforeFall = heightTracker.playerRb.position.y;
        lastHeightCheckpoint = heightTracker.playerRb.position.y;
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
            Debug.Log($"Started falling from height: {highestPointBeforeFall}");
        }

        if (isFalling && !hasLanded && Mathf.Abs(verticalVelocity) < velocityThreshold)
        {
            isFalling = false;
            hasLanded = true;

            float fallDistanceMeters = (highestPointBeforeFall - currentHeight) / unityUnitsPerMeter;
            Debug.Log($"Landed. Fall Distance: {fallDistanceMeters:F2} meters");

            if (fallDistanceMeters >= bigFallThreshold)
            {
                IncreaseFrustration(1f);
                Debug.Log("Big Fall detected!");
            }

            if (fallDistanceMeters >= minFallDistanceForRepeat)
            {
                if (Mathf.Abs(currentHeight - lastFallHeight) < 0.1f)
                {
                    consecutiveFallCount++;
                    Debug.Log($"Repeated Fall Count: {consecutiveFallCount}");
                    if (consecutiveFallCount >= repeatFallThreshold)
                    {
                        IncreaseFrustration(2f);
                        Debug.Log("Repeated Falls threshold reached!");
                    }
                }
                else
                {
                    lastFallHeight = currentHeight;
                    consecutiveFallCount = 1;
                    Debug.Log("Repeated Falls count reset.");
                }
            }
        }

        float heightCheckpointMeters = (currentHeight - lastHeightCheckpoint) / unityUnitsPerMeter;

        if (heightCheckpointMeters >= heightCheckpointInterval)
        {
            Debug.Log($"New Height condition met at: {currentHeight}");
        }

        if (currentHeight > highestPointBeforeFall)
            highestPointBeforeFall = currentHeight;
    }

    public void UpdateLastHeightCheckpoint()
    {
        lastHeightCheckpoint = CurrentHeight;
        Debug.Log($"Height checkpoint explicitly updated to: {lastHeightCheckpoint}");
    }

    public void IncreaseFrustration(float amount)
    {
        frustrationLevel += amount;
        Debug.Log($"Frustration increased to {frustrationLevel}");
    }

    public void DecreaseFrustration(float amount)
    {
        frustrationLevel = Mathf.Max(0f, frustrationLevel - amount);
        Debug.Log($"Frustration decreased to {frustrationLevel}");
    }

    public float HighestPointBeforeFall => highestPointBeforeFall;
    public float LastHeightCheckpoint => lastHeightCheckpoint;
    public int ConsecutiveFallCount => consecutiveFallCount;
    public float FrustrationLevel => frustrationLevel;
    public float CurrentHeight => heightTracker.playerRb.position.y;
}
