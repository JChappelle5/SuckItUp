using UnityEngine;
using UnityEngine.UI;

public class TestTriggerScript : MonoBehaviour
{
    public GameObject interactionPrompt;  // "Press E" UI
    public GameObject tutorialPopup;      // Tutorial popup panel

    public bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            tutorialPopup.SetActive(true);
            Time.timeScale = 0f;
        }

        // Check if the player presses Space
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed! Closing tutorial popup.");
            interactionPrompt.SetActive(false);  // Hide the popup
            tutorialPopup.SetActive(false);
            Time.timeScale = 1f;
        }
    }
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
