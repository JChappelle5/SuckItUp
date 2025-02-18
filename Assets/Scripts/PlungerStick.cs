using UnityEngine;

public class PlungerStick : MonoBehaviour
{
    public LayerMask stickableLayer;
    public Rigidbody2D playerRb;

    public bool isStuck = false;
    private Vector2 stuckPosition;
    private Rigidbody2D plungerRb;
    private Collider2D stuckSurface;

    void Start()
    {
        plungerRb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & stickableLayer) != 0) // Check if it's a valid surface
        {
            Debug.Log("Plunger colliding with: " + collision.gameObject.name);

            if (Input.GetMouseButton(0)) // Stick only if left-click is held
            {
                StickToSurface(collision.collider);
            }
        }
    }

    void Update()
    {
        // Detect if the player releases the left mouse button while stuck
        if (isStuck && Input.GetMouseButtonUp(0))
        {
            ReleasePlunger();
        }

        // Debug collision detection (useful for testing)
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(0.2f, 1.0f), transform.eulerAngles.z, stickableLayer);

        if (hit != null)
        {
            Debug.Log("Plunger SHOULD be colliding with: " + hit.gameObject.name);
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (isStuck && other == stuckSurface) // Release if moving away from stuck object
        {
            ReleasePlunger();
        }
    }
    void StickToSurface(Collider2D hit)
    {
        Debug.Log("Plunger Stuck to: " + hit.gameObject.name);

        isStuck = true;
        stuckSurface = hit;

        // Cast a ray from the plunger's position toward its movement direction
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, 1f, stickableLayer);

        if (hitInfo.collider != null)
        {
            // Stick exactly at the contact point of the raycast
            transform.position = hitInfo.point;
        }

        // Stop plunger movement completely
        plungerRb.linearVelocity = Vector2.zero;
        plungerRb.angularVelocity = 0f;
        plungerRb.bodyType = RigidbodyType2D.Kinematic;
        plungerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Stop player movement while stuck
        playerRb.linearVelocity = Vector2.zero;
        playerRb.gravityScale = 0;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // STOP ROTATION BY SETTING `rotationSpeed` TO 0
        PlungerAim plungerAim = FindObjectOfType<PlungerAim>(); // Find the script
        if (plungerAim != null)
        {
            plungerAim.rotationSpeed = 0; // Stop rotation
        }
    }






    void ReleasePlunger()
    {
        Debug.Log("Plunger Released!");

        isStuck = false;
        stuckSurface = null;

        // Restore plunger movement
        plungerRb.bodyType = RigidbodyType2D.Dynamic;
        plungerRb.linearVelocity = Vector2.zero;
        plungerRb.angularVelocity = 0f; //  Reset any unwanted spin
        plungerRb.constraints = RigidbodyConstraints2D.FreezeRotation; // Reapply freeze rotation to prevent rolling

        // Restore player movement
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.gravityScale = 1;
        playerRb.linearVelocity = Vector2.zero; //  Ensure player starts fresh
        playerRb.angularVelocity = 0f; //  Reset any rotation forces
        playerRb.freezeRotation = true; //  Prevent the player from spinning

        //  Apply a small downward force to ensure gravity re-engages
        playerRb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);

        // Debugging logs
        Debug.Log($"After Release - Player Velocity: {playerRb.linearVelocity}, Gravity Scale: {playerRb.gravityScale}");
        Debug.Log($"After Release - Plunger Velocity: {plungerRb.linearVelocity}, Gravity Scale: {plungerRb.gravityScale}");

        // Restore rotation speed for aiming
        PlungerAim plungerAim = FindFirstObjectByType<PlungerAim>();
        if (plungerAim != null)
        {
            plungerAim.rotationSpeed = 500; // Restore original rotation speed
        }
    }






}
