using UnityEngine;

public class Angel : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float startPos = -3.75f;
    public bool trackingHeight = true;
    public Vector2 lastPos;
    public LayerMask stickableSurfaceLayer;
    public Transform bottomDetector;
    public float groundCheckRadius = 0.1f;
    private bool wasGrounded = false;
    public float timeAirborne = 0f;

    void Update()
    {
        bool isCurrentlyGrounded = IsGrounded();

        if(timeAirborne >= 1f)
        {
            
        }

        if (isCurrentlyGrounded && !wasGrounded && IsUpright()) // Updates last position once the player is grounded and upright
        {
            lastPos = playerRb.position;
        }
        else if (!isCurrentlyGrounded && wasGrounded) // Tracks that player is airborne
        {
            Debug.Log("Player is airborne.(Angel)");
            timeAirborne += Time.deltaTime;
        }
        wasGrounded = isCurrentlyGrounded;

        if(!isCurrentlyGrounded)
        {
            timeAirborne += Time.deltaTime;
        }
    }

    bool IsUpright() // Checks if player is standing straight (avoids player being brought up to a wall)
    {
        Vector2 facingUp = (Vector2) playerRb.transform.up;
        if(Vector2.Dot(facingUp, Vector2.up) > 0.9f)
            return true;
        else
            return false;
    }

    bool IsGrounded() // Checks if player is on ground properly
    {
        Collider2D hit = Physics2D.OverlapCircle(bottomDetector.position, groundCheckRadius, stickableSurfaceLayer);
        Debug.DrawRay(bottomDetector.position, Vector2.down * groundCheckRadius, hit != null ? Color.green : Color.red);
        return hit != null;
    }
}
