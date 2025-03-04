using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRageEvents : MonoBehaviour
{
    [Header("References")]
    public NarratorManager narratorManager; // Assign your NarratorManager GameObject here

    [Header("Fall Thresholds")]
    public float mildFallThreshold = 0.3f;
    public float majorFallThreshold = 1.0f;

    [Header("Repeated Falls Settings")]
    public float repeatedFallWindow = 10f;
    public int repeatedFallCountThreshold = 3;

    [Header("Climbing Settings")]
    public float climbingCheckInterval = 2f;
    [Range(0f, 1f)]
    public float climbingChance = 0.2f;

    private Rigidbody2D rb;

    private float previousHeight;
    private float maxHeightReached;
    private float potentialMaxHeight = 0f;

    private float lastFallTime;
    private int fallsInWindow;

    private float climbingTimer;
    private bool hasFallenAtLeastOnce = false;

    private float narrationCooldown = 0f;
    private float narrationCooldownDuration = 1.5f;

    public float bufferHeightMargin = 0.5f;
    private bool isFalling = false;
    private float highestPointBeforeFall;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        previousHeight = transform.position.y;
        maxHeightReached = previousHeight;
        lastFallTime = -repeatedFallWindow;
        fallsInWindow = 0;
        climbingTimer = climbingCheckInterval;
    }

    public float heightMargin = 1f;
    public float newHeightCooldownDuration = 2f;
    private float newHeightCooldownTimer = 0f;

    void Update()
    {
        float currentHeight = transform.position.y;

        if (rb.linearVelocity.y < 0 && !isFalling)
        {
            isFalling = true;
            highestPointBeforeFall = currentHeight;
        }

        if (newHeightCooldownTimer > 0f)
            newHeightCooldownTimer -= Time.deltaTime;

        if (currentHeight > maxHeightReached + bufferHeightMargin)
        {
            potentialMaxHeight = currentHeight;
        }

        previousHeight = currentHeight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.linearVelocity.y == 0 && isFalling)
        {
            isFalling = false;
            float landingHeight = transform.position.y;
            float fallDistance = highestPointBeforeFall - landingHeight;

            if (landingHeight > maxHeightReached)
            {
                maxHeightReached = landingHeight;
                narratorManager.PlayRandomClip(narratorManager.newHeightClips);
                newHeightCooldownTimer = newHeightCooldownDuration;
                narrationCooldown = Time.time + narrationCooldownDuration;
            }

            if (Time.time < narrationCooldown)
                return;

            if (fallDistance >= mildFallThreshold)
            {
                hasFallenAtLeastOnce = true;

                if (fallDistance >= majorFallThreshold)
                {
                    narratorManager.PlayRandomClip(narratorManager.majorFallClips);
                }
                else
                {
                    narratorManager.PlayRandomClip(narratorManager.mildFallClips);
                }

                narrationCooldown = Time.time + narrationCooldownDuration;
            }
        }
    }
}
