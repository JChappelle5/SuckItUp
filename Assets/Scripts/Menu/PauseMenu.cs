using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject pauseButtons;
    public GameObject resetMenu;
    public GameObject quitImage;
    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false); // Hide the pause menu at the start
        isPaused = false; 
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        Time.timeScale = 1f; // Resume game
        SceneManager.LoadScene(1); // Reload current scene
    }
}
