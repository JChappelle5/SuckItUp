using UnityEngine;

public class PlungerStick : MonoBehaviour
{
    public LayerMask stickableLayer;
    public float stickRange = 1.5f;
    public Rigidbody2D playerRb;

    public bool isStuck = false;
    private Vector2 stuckPosition;
    private Rigidbody2D plungerRb;
    private Collider2D stuckSurface;

    void Start()
    {
        plungerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if plunger is touching a valid surface
        Collider2D hit = Physics2D.OverlapCircle(transform.position, stickRange, stickableLayer);

        if (hit != null)
        {
            Debug.Log("Plunger touching: " + hit.gameObject.name);
        }

        // FIX: Only check hit.GetComponent<Collider>() if hit is NOT null
        if (Input.GetMouseButtonDown(0) && hit != null)
        {
            StickToSurface(hit);
        }

        // Release if the left mouse button is released
        if (Input.GetMouseButtonUp(0) && isStuck)
        {
            ReleasePlunger();
        }
    }


    void StickToSurface(Collider2D hit)
    {
        Debug.Log("Plunger Stuck to: " + hit.gameObject.name);

        isStuck = true;
        stuckSurface = hit; // Store the surface we are stuck to

        // Use ClosestPoint() to get the exact sticking position on the edge
        stuckPosition = hit.ClosestPoint(transform.position);

        // Move plunger to the sticking position
        transform.position = stuckPosition;

        // Stop plunger movement
        plungerRb.linearVelocity = Vector2.zero;
        plungerRb.bodyType = RigidbodyType2D.Static;

        // Stop the player from moving while stuck
        playerRb.linearVelocity = Vector2.zero;
        playerRb.gravityScale = 0;
        playerRb.constraints = RigidbodyConstraints2D.FreezePosition;
    }



    void ReleasePlunger()
    {
        Debug.Log("Plunger Released!");

        isStuck = false;
        stuckSurface = null;

        // Allow plunger movement again
        plungerRb.bodyType = RigidbodyType2D.Dynamic;

        // Unfreeze the player and allow movement again
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.gravityScale = 1;
    }
}
