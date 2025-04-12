using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject pauseButtons;
    public GameObject resetMenu;
    public GameObject quitImage;
    public static bool isPaused;
    public Rigidbody2D rb;
    private GameObject player;
    private PlungerMovement flingScript;
    public const string SAVE_FILENAME = "/savefile.json";
    public SaveSystem saveSystem;
    public Vector2 spawnPosition = new Vector2(-21.908f, -4.362f); // Default spawn position

    void Start()
    {
        pauseMenu.SetActive(false); // Hide the pause menu at the start
        isPaused = false; 
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player object
        flingScript = player.GetComponent<PlungerMovement>(); // Get the PlayerScript component
    }

    // Update is called once per frame
    void Update()
    {
        // Resume or pause depending on current state of game
        if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame(); 
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            ResumeGame(); 
        }
    }

    public void PauseGame()
    {
        // Hide menus/buttons if they were closed while opened
        settingsMenu.SetActive(false); 
        resetMenu.SetActive(false); 
        quitImage.SetActive(false); 

        pauseMenu.SetActive(true); // Show pause menu
        pauseButtons.SetActive(true);
        Time.timeScale = 0f; // Pause game
        isPaused = true; 
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false); // Hide menu
        Time.timeScale = 1f; // Resume game
        isPaused = false;
    }

    public void ReturnToMenu()
    {
        saveSystem.SaveData(); // Save game before returning to menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        Time.timeScale = 1f; // Resume game
        saveSystem.SaveData(spawnPosition); // Save game at spawn position
        SceneManager.LoadScene(1); // Reload current scene
    }

    public void UnstuckPlayer()
    {
        // Checks if player is not moving and not grounded
        if(!flingScript.isCurrentlyGrounded && rb.linearVelocity.magnitude < 0.01f)
        {
            Debug.Log("Player is stuck");
            rb.AddForce(new Vector2(1,1) * 250); // Apply upward force to unstuck player
            ResumeGame();
        }
        else
        {
            Debug.Log("Player is not stuck");
        }
    }
}
