using UnityEngine;
using TMPro;

public class HeightTracker : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public TMP_Text heightText;
    public float startPos = -3.75f;
    public bool trackingHeight = true;

    void Update()
    {
        // always update to display height
        if(trackingHeight)
        {
            displayHeight(startPos);
        }
    }

    void displayHeight(float displayedHeight)
    {
        if(playerRb == null) return; // stops error if player isn't attatched

        int meters = Mathf.FloorToInt(playerRb.position.y / 5.45f); // does initial check for current height
        if(meters == -1) // bottom of level (-2m)
            meters = 0;
        else if(meters == 0) // 1 meter up (-1m)
            meters = 1;
        else
            meters = Mathf.FloorToInt(playerRb.position.y / 5.45f) + 1; // gets # of meters + 1 to account for the -1

        heightText.text = string.Format("{0}m", meters); // displays height i.e. 1m, 5m, 10m, etc.
    }
}
