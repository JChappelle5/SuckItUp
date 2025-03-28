using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Loads game scene
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game
    }
}
