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
            Vector2 playerVelocity = collision.relativeVelocity; // Get velocity at point of collision

            bounceForce = new Vector2(playerVelocity.x * 2f, Mathf.Abs(playerVelocity.magnitude) * 0.85f); // Calculate bounce force based on relative velocity

            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse); // Apply bounce force to player
        }
    }
}
