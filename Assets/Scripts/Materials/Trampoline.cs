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

            Vector2 playerVelocity = rb.linearVelocity; // Get velocity at point of collision

            Debug.Log($"Trampoline Contact - Velocity = {playerVelocity}");

            bounceForce = new Vector2(playerVelocity.x * 4f, Mathf.Abs(playerVelocity.magnitude) * 1.3f); // Calculate bounce force based on velocity of player at time of collision

            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse); // Apply bounce force to player
        }
    }
}
