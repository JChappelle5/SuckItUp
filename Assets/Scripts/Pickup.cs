using UnityEngine;

public class Pickup : MonoBehaviour
{
    // This function is called when another collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the item after it's collision triggered
        Destroy(gameObject);
    }
}