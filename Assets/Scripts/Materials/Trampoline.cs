using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Trampoline : MonoBehaviour
{
    private Vector2 bounceForce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player is colliding with trampoline
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            // Get the velocity at the point of collision
            Vector2 playerVelocity = rb.linearVelocity;

            //caps magnitude to be at 1
            float myValue;
            myValue = Mathf.Clamp(playerVelocity.magnitude * 0.075f, 0f, 0.99f);

            // Calculate the bounce force based on the player's velocity magnitude and velocity x
            float bounceForce = ((float)(Mathf.Abs(playerVelocity.x) * 5.25) * myValue); // Adjusted bounce force

            // Set a cap for the maximum vertical bounce force (vertical velocity)
            float maxBounceForce = 75f; 
            bounceForce = Mathf.Min(bounceForce, maxBounceForce);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
    }
}
