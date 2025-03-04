using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public float timeRemaining = 0;
    public bool timeIsRunning = true;
    public TMP_Text timeText;

    void Update()
    {
        if(timeIsRunning)
        {
            timeRemaining += Time.deltaTime;
            displayTime(timeRemaining);
        }
    }

    void displayTime(float displayedTime)
    {
        float hours = Mathf.FloorToInt(displayedTime / 3600);
        float minutes = Mathf.FloorToInt((displayedTime % 3600) / 60);
        float seconds = Mathf.FloorToInt(displayedTime % 60);
        if (hours > 0)
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
