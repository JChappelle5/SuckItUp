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

        // Change direction player is facing depending on key (A/D)
        if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.flipX = true; // Face left
        }

        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.flipX = false; // Face right
        }
    }
}
