using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class ToiletWin : MonoBehaviour
{
    public GameObject interactionPrompt;
    public bool isPlayerNearby = false; 
    private GameObject UICanvas;
    public GameObject winCanvas;
    public Stopwatch timer;
    public TMP_Text finalTimeText;

    void Start()
    {
        UICanvas = GameObject.Find("Timer");
        timer = UICanvas.GetComponent<Stopwatch>();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            timer.timeIsRunning = false;
            Time.timeScale = 0f;
            interactionPrompt.SetActive(false);
            winCanvas.SetActive(true);
            displayFinalTime();
        }
    }

    void displayFinalTime()
    {
        float hours = Mathf.FloorToInt(timer.timeElapsed / 3600);
        float minutes = Mathf.FloorToInt((timer.timeElapsed % 3600) / 60);
        float seconds = Mathf.FloorToInt(timer.timeElapsed % 60);

        if(hours > 0)
            finalTimeText.text = string.Format("Final Time: {0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            finalTimeText.text = string.Format("Final Time: {0:00}:{1:00}", minutes, seconds);
    }

    // Prompt User When Near
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered trigger: " + other.name + " with tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Showing Press E prompt.");
            interactionPrompt.SetActive(true);
            isPlayerNearby = true;
        }
    }

    // Get rid of prompt when out of range
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exited trigger: " + other.name + " with tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            if (interactionPrompt != null)
            {
                Debug.Log("Player left the area. Hiding Press E prompt.");
                interactionPrompt.SetActive(false);
                isPlayerNearby = false;
            }
        }
    }
}
