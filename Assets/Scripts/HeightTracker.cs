using UnityEngine;
using TMPro;

public class HeightTracker : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public TMP_Text heightText;
    public bool trackingHeight = true;

    // We'll store the final calculated meter value
    private int currentMeters = 0;

    void Update()
    {
        if (trackingHeight && playerRb != null)
        {
            DisplayHeight();
        }
    }

    void DisplayHeight()
    {
        // Convert Y position to “meters” based on your ratio (1 meter ~ 5.235 units)
        int meters = Mathf.FloorToInt(playerRb.position.y / 5.235f);

        // The custom logic you had: if meters == -1 => 0, etc.
        if (meters == -1)
            meters = 0;
        else if (meters == 0)
            meters = 1;
        else
            meters = Mathf.FloorToInt(playerRb.position.y / 5.235f) + 1;

        currentMeters = meters;
        heightText.text = $"{meters}m";
    }

    // Public getter so other scripts (like MeterBasedZoneDetector) can read it
    public int GetCurrentMeters()
    {
        return currentMeters;
    }
}
