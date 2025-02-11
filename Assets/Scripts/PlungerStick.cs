using UnityEngine;

public class PlungerStick : MonoBehaviour
{
    public LayerMask stickableLayer;
    public float stickRange = 1.5f;
    public Rigidbody2D playerRb;
    public float launchForce = 15f;

    public bool isStuck = false;
    private Vector2 stuckPosition;
    private Rigidbody2D plungerRb;

    void Start()
    {
        plungerRb = GetComponent<Rigidbody2D>(); // Get Rigidbody for the plunger
    }

    void Update()
    {
        // ?? Stick ONLY if player is actively holding the left mouse button
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, stickRange, stickableLayer);
            if (hit.collider != null && !isStuck) // Only stick if not already stuck
            {
                Debug.Log("Plunger Stuck to: " + hit.collider.gameObject.name);
                isStuck = true;
                stuckPosition = hit.point;
                transform.position = hit.point;

                // ?? Stop the plunger from moving
                plungerRb.linearVelocity = Vector2.zero;
                plungerRb.bodyType = RigidbodyType2D.Static; // Make plunger stop moving

                // ?? Stop the player from moving while hanging onto the plunger
                playerRb.linearVelocity = Vector2.zero;
                playerRb.gravityScale = 0;
                playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY; // Prevent movement
            }
        }
        else if (isStuck) // ?? Release plunger when left mouse is released
        {
            ReleasePlunger();
        }
    }

    void ReleasePlunger()
    {
        Debug.Log("Plunger Released!");

        isStuck = false;
        plungerRb.bodyType = RigidbodyType2D.Dynamic; // Allow plunger movement again

        // ?? Unfreeze the player and allow movement again
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.gravityScale = 1; // Restore gravity

        Vector2 launchDirection = (playerRb.position - stuckPosition).normalized;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
    }
}
