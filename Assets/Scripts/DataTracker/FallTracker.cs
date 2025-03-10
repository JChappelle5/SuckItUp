using UnityEngine;

public class FallTracker : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFalling = false;
    private float fallStartY;

    [Header("1 �Game Meter� in Unity Units")]
    // Adjust if you change your meter ratio. 
    [SerializeField] private float unityUnitsPerMeter = 5.235f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if velocity is negative => we�re falling
        if (rb.linearVelocity.y < -0.1f) // small threshold to ignore tiny vertical movement
        {
            if (!isFalling)
            {
                isFalling = true;
                fallStartY = transform.position.y;
            }
        }
        else
        {
            // If we were falling, but now velocity is >= -0.1 => we landed or stopped
            if (isFalling)
            {
                isFalling = false;

                float fallEndY = transform.position.y;
                float distanceFallenUnits = fallStartY - fallEndY; // raw Unity units

                // Only record if it�s a meaningful drop
                if (distanceFallenUnits > 0.2f)
                {
                    // Convert to game meters before logging
                    float distanceFallenMeters = distanceFallenUnits / unityUnitsPerMeter;

                    // Now we record the fall in meters
                    LocalDataLogger.Instance.RecordFall(distanceFallenMeters);
                }
            }
        }
    }
}
