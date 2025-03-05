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

    void Update()
    {
        bool isCurrentlyGrounded = IsGrounded();

        if (isCurrentlyGrounded && !wasGrounded && IsUpright())
        {
            lastPos = playerRb.position;
        }
        else if (!isCurrentlyGrounded && wasGrounded)
        {
            Debug.Log("Player is airborne.(Angel)");
        }
        wasGrounded = isCurrentlyGrounded;
    }

    bool IsUpright()
    {
        Vector2 facingUp = (Vector2) playerRb.transform.up;
        if(Vector2.Dot(facingUp, Vector2.up) > 0.9f)
            return true;
        else
            return false;
    }

    bool IsGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(bottomDetector.position, groundCheckRadius, stickableSurfaceLayer);
        Debug.DrawRay(bottomDetector.position, Vector2.down * groundCheckRadius, hit != null ? Color.green : Color.red);
        return hit != null;
    }
}
