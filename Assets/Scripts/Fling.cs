using UnityEngine;

public class PlungerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rotationSpeed = 50f;
    public float airRotationSpeed = 100f;
    public float maxPullBack = 45f;
    public float minLaunchForce = 5f;
    public float maxLaunchForce = 20f;
    private float leanAngle = 0f;

    private bool isCharging = false;    // Detect if pulling back
    private bool isStickingToWall = false;
    public LayerMask stickableSurfaceLayer;
    public Transform bottomDetector;
    public float normalFactor = 1f;
    public float slowdownFactor = 0.75f;
    private bool wasGrounded = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isCurrentlyGrounded = IsGrounded();

        // Debug print when grounded state changes
        if (isCurrentlyGrounded && !wasGrounded)
        {
            Debug.Log("Player is grounded.");
        }
        else if (!isCurrentlyGrounded && wasGrounded)
        {
            Debug.Log("Player is airborne.");
        }
        wasGrounded = isCurrentlyGrounded;

        // Handle movement
        if ((isCurrentlyGrounded || isStickingToWall) && Input.GetKey(KeyCode.Space))
        {
            HandleLeaning();
        }
        else if (!isCurrentlyGrounded && !isStickingToWall)
        {
            HandleAirRotation();
        }

        if (Input.GetKeyUp(KeyCode.Space)) // Release to launch
        {
            //regularSpeedMotion();
            Launch();
        }
    }

    //void regularSpeedMotion()
    //{
    //    Time.timeScale = normalFactor;
    //    Time.fixedDeltaTime = Time.timeScale * 0.02f;
    //}
    //void slowSpeedMotion()
    //{
    //    Time.timeScale = slowdownFactor;
    //    Time.fixedDeltaTime = Time.timeScale * 0.02f;
    //}

    void HandleLeaning()
    {
        float input = -Input.GetAxisRaw("Horizontal");
        if (input != 0)
        {
            isCharging = true;
            leanAngle += input * rotationSpeed * Time.deltaTime;
            leanAngle = Mathf.Clamp(leanAngle, -maxPullBack, maxPullBack);
            rb.MoveRotation(leanAngle);
        }
    }

    void HandleAirRotation()
    {
        float input = -Input.GetAxisRaw("Horizontal");
        if (input != 0)
        {
            float rotation = input * airRotationSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation + rotation);
        }
    }

    void Launch()
    {
        if (!isCharging) return;
        //slowSpeedMotion();

        float chargePercent = Mathf.Abs(leanAngle) / maxPullBack;
        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, chargePercent);

        float angleRad = leanAngle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

        // Reset
        isCharging = false;
        isStickingToWall = false;
        leanAngle = 0f;
        rb.MoveRotation(leanAngle);

        //  Reset Gravity and Unfreeze Movement
        rb.gravityScale = 3;
        rb.constraints = RigidbodyConstraints2D.None;
    }

    bool IsGrounded()
    {
        float rayLength = 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, stickableSurfaceLayer);
        Debug.DrawRay(transform.position, Vector2.down * rayLength, hit.collider != null ? Color.green : Color.red);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & stickableSurfaceLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y <= bottomDetector.position.y)
                {
                    StickToWall();
                    break;
                }
            }
        }
    }

    private void StickToWall()
    {
        Debug.Log("Player is sticking to the wall!");
        isStickingToWall = true;
        rb.linearVelocity = Vector2.zero;

        //  Freeze Gravity While Sticking to Wall
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }
}
