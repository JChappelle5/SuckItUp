using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation
    private GameObject player; // Reference to the player
    private Rigidbody2D playerRb; // Player's Rigidbody2D

    void Update()
    {
        // Rotate around the Z-axis
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "BottomDetector") // Ensure only BottomDetector triggers this
        {
            player = collision.transform.parent.gameObject; // Get the player object
            playerRb = player.GetComponent<Rigidbody2D>(); // Get player's Rigidbody2D

            if (playerRb != null)
            {
                playerRb.gravityScale = 0f; // Disable gravity
                playerRb.linearVelocity = Vector2.zero; // Stop movement
            }

            player.transform.SetParent(transform); // Attach player to the rotating platform
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "BottomDetector") // Ensure only BottomDetector triggers exit
        {
            Debug.Log("Player left the platform, resetting parent.");
            Invoke(nameof(ResetPlayerParent), 0.01f); // Small delay to avoid physics issues
        }
    }

    void ResetPlayerParent()
    {
        if (player != null && player.transform.parent == transform)
        {
            if (playerRb != null)
            {
                playerRb.gravityScale = 1f; // Reset gravity
            }
            player.transform.SetParent(null);
            player = null;
            playerRb = null;
        }
    }
}
