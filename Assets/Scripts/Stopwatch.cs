using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public float timeElapsed = 0f;  // Counts up from 0
    public bool timeIsRunning = true;
    public TMP_Text timeText;

    void Update()
    {
        if (timeIsRunning)
        {
            timeElapsed += Time.deltaTime;
            DisplayTime(timeElapsed);
        }
    }

    // Update the on-screen text
    void DisplayTime(float displayedTime)
    {
        float hours = Mathf.FloorToInt(displayedTime / 3600);
        float minutes = Mathf.FloorToInt((displayedTime % 3600) / 60);
        float seconds = Mathf.FloorToInt(displayedTime % 60);

        if (hours > 0)
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Public method so other scripts can get the same mm:ss (or hh:mm:ss)
    public string GetFormattedTime()
    {
        float hours = Mathf.FloorToInt(timeElapsed / 3600);
        float minutes = Mathf.FloorToInt((timeElapsed % 3600) / 60);
        float seconds = Mathf.FloorToInt(timeElapsed % 60);

        if (hours > 0)
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
