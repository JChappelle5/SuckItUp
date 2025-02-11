using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the player's sprite renderer
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if mouse is to the left or right of the player
        if (mousePosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Face left
        }
        else
        {
            spriteRenderer.flipX = false; // Face right
        }
    }
}
