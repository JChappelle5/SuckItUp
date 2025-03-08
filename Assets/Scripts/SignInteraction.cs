using UnityEngine;
using UnityEngine.UI;

public class SignInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;  // "Press E" UI
    public GameObject tutorialPopup;      // Tutorial popup panel

    public bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            tutorialPopup.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger: " + other.name + " with tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Showing Press E prompt.");
            interactionPrompt.SetActive(true);
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger: " + other.name);  // Log anything that exits
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the area. Hiding Press E prompt.");
            //interactionPrompt.SetActive(false);
            //isPlayerNearby = false;
        }
    }




    //public void ClosePopup()
    //{
    //    tutorialPopup.SetActive(false);
    //}
}
