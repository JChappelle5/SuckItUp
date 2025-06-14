using UnityEngine;
using System.Collections;

public class PlungerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rotationSpeed = 50f;
    public float maxPullBack = 45f;
    public float minLaunchForce = 5f;
    public float maxLaunchForce = 20f;
    public static bool isCharging = false;
    public bool isStickingToWall = false;
    public LayerMask stickableSurfaceLayer;
    public LayerMask slimeLayer;
    public Transform bottomDetector;
    public float groundCheckRadius = 0.1f; // For ground check
    public float wallCheckRadius = 0.1f;   // For wall check
    private bool wasGrounded = true;
    private float storedLeanAngle = 0f;
    public float normalFactor = 1f;
    public float slowdownFactor = 0.7f;
    public Sprite PlayerStanding;
    public Sprite PlayerSticking;
    public Sprite PlayerLeft1;
    public Sprite PlayerLeft2;
    public Sprite PlayerLeft3;
    public Sprite PlayerLeft4;
    public Sprite PlayerRight1;
    public Sprite PlayerRight2;
    public Sprite PlayerRight3;
    public Sprite PlayerRight4;
    public bool TimerOn = false;
    public float stickTime;
    public bool isCurrentlyGrounded = false;
    public GameObject tilemap;
    public GameObject slimeTilemap;
    private bool isOnSlime = false;
    private bool isOnMovingPlatform = false;
    public float wedgeTimer = 0f;
    private int moveKeyPressCount = 0;
    private float moveKeyPressWindow = 5f; // Track presses over 5 seconds
    public Transform leftDetector;
    public Transform rightDetector;
    public Transform topDetector;
    public bool isCurrentlyOnSide;

    void Awake()
    {
        stickTime = 3f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!PauseMenu.isPaused) // Check if game is paused
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                moveKeyPressCount++;
            }

            isCurrentlyGrounded = IsGrounded();
            checkOnFloor();

            isCurrentlyOnSide = IsOnSide();

            // Debug print when grounded state changes
            if (isCurrentlyGrounded && !wasGrounded)
            {
                regularSpeedMotion();
                //Debug.Log("Player is grounded.");
            }
            else if (!isCurrentlyGrounded && wasGrounded)
            {
                slowSpeedMotion();
                //Debug.Log("Player is airborne.");
            }
            wasGrounded = isCurrentlyGrounded;

            if (!isCharging && !isStickingToWall && wasGrounded)
            {
                storedLeanAngle = 0f;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;
            }

            if (isCurrentlyGrounded && !isRotatedOnWall())
            {
                TimerOn = false;
                stickTime = 3f;
            }

            if (Input.GetKeyUp(KeyCode.Space) && (rb.linearVelocity.magnitude < 0.01f) && !isCharging) // Reset if not charging
            {
                storedLeanAngle = 0f;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;
            }

            if (Input.GetKeyUp(KeyCode.Space) && (rb.linearVelocity.magnitude < 0.01f) && isCharging) // Release to launch
            {
                Launch();
            }

            if (Input.GetKeyUp(KeyCode.Space) && Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer) && isCharging)
            {
                Launch();
            }

            if (TimerOn && isStickingToWall)
            {
                if (stickTime > 0)
                {
                    stickTime -= Time.deltaTime;
                }
                else
                {
                    unstickPlayer();
                    TimerOn = false;
                    stickTime = 3f;
                }
            }
            if (isOnSlime)
            {
                transform.position += Vector3.down * Time.deltaTime * .8f;
            }
            if (isStickingToWall && !Input.GetKey(KeyCode.Space))
            {
                unstickPlayer();
            }

        }
    }

    void FixedUpdate()
    {
        // Handle movement
        if ((isCurrentlyGrounded || isStickingToWall) && Input.GetKey(KeyCode.Space) && ((Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D))))
        {
            HandleCharging();
        }
        else if (!isCurrentlyGrounded && !isStickingToWall)
        {
            HandleAirRotation();
        }
    }

    void HandleCharging()
    {
        if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            rb.linearVelocity = Vector2.zero;
        }
        else if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.linearVelocity = new Vector2(0, -1);
        }

        float input = -Input.GetAxisRaw("Horizontal");

        if (input != 0)
        {
            if (!isCharging)
            {
                storedLeanAngle = 0f;
            }

            isCharging = true;
            storedLeanAngle += input * rotationSpeed * Time.fixedDeltaTime;
            storedLeanAngle = Mathf.Clamp(storedLeanAngle, -maxPullBack, maxPullBack);

            float lowCharge = maxPullBack * 0.33f;
            float medCharge = maxPullBack * 0.66f;

            if (storedLeanAngle == maxPullBack)
            { //Max right charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerRight4;
            }
            else if (storedLeanAngle > medCharge)
            { //Large right charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerRight3;
            }
            else if (storedLeanAngle > lowCharge)
            { //Medium right charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerRight2;
            }
            else if (storedLeanAngle > 0)
            { //Low right charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerRight1;
            }
            else if (storedLeanAngle == -maxPullBack)
            { //Max left charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerLeft4;
            }
            else if (storedLeanAngle < -medCharge)
            { //Large left charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerLeft3;
            }
            else if (storedLeanAngle < -lowCharge)
            { //Medium left charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerLeft2;
            }
            else if (storedLeanAngle < 0)
            { //Low left charge
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerLeft1;
            }
        }
    }

    void HandleAirRotation()
    {
        storedLeanAngle = 0f;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;

        float input = -Input.GetAxisRaw("Horizontal");
        if (input != 0)
        {
            float maxRotationPerFrame = 10.15f;
            float rotation = Mathf.Clamp(input * 10.15f, -maxRotationPerFrame, maxRotationPerFrame);

            rb.MoveRotation(rb.rotation + rotation);
        }
    }

    void Launch()
    {
        if (!isCharging) return;

        //if ((Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer)
        //    || Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer))  && isRotatedOnWall())
        //{
        //    isStickingToWall = true;  // Force stick state if we're actually on wall
        //}

        Debug.Log($"Launch - Charge: {storedLeanAngle}, IsSticking: {isStickingToWall}, Rotation: {rb.rotation}");

        this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;

        float chargePercent = Mathf.Abs(storedLeanAngle) / maxPullBack;
        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, chargePercent);
        float input = -Input.GetAxisRaw("Horizontal");
        float angleRad = storedLeanAngle * Mathf.Deg2Rad;

        Vector2 launchDirection = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;
        Vector2 upWallLaunchDir, downWallLaunchDir;

        rb.gravityScale = 10; // Reset gravity
        rb.constraints = RigidbodyConstraints2D.None; // Unfreeze movement
        rb.linearVelocity = Vector2.zero; // Reset velocity

        if (isStickingToWall)
        {
            float rotation = rb.rotation % 360;

            Debug.Log($"Wall Launch - Rotation: {rotation}, Input: {input}, LaunchForce: {launchForce}");

            if ((rotation > 240 && rotation < 300) || (rotation < -60 && rotation > -120)) // on right wall
            {
                upWallLaunchDir = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;
                downWallLaunchDir = new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)).normalized;

                if (storedLeanAngle < 0) // facing upwards
                {
                    rb.AddForce(upWallLaunchDir * launchForce, ForceMode2D.Impulse);
                }
                else if (storedLeanAngle > 0)
                {
                    rb.AddForce(downWallLaunchDir * launchForce, ForceMode2D.Impulse);
                }
            }
            else if ((rotation > 60 && rotation < 120) || (rotation < -240 && rotation > -300)) // on left wall
            {
                upWallLaunchDir = new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)).normalized;
                downWallLaunchDir = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;

                if (storedLeanAngle < 0) // facing upwards
                {
                    rb.AddForce(upWallLaunchDir * launchForce, ForceMode2D.Impulse);
                }
                else if (storedLeanAngle > 0)
                {
                    rb.AddForce(downWallLaunchDir * launchForce, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        }

        // Reset
        isCharging = false;
        isStickingToWall = false;
        isOnSlime = false;
        storedLeanAngle = 0f;
    }

    //  Ground Detection Using OverlapCircle
    public bool IsGrounded()
    {
        Collider2D slimeHit = Physics2D.OverlapCircle(bottomDetector.position, groundCheckRadius, slimeLayer);
        bool groundBelow = Physics2D.OverlapCircle(bottomDetector.position, groundCheckRadius, stickableSurfaceLayer | slimeLayer);


        return groundBelow || slimeHit;
    }

    public bool IsOnSide()
    {
        bool groundLeft = Physics2D.OverlapCircle(leftDetector.position, groundCheckRadius, stickableSurfaceLayer | slimeLayer);
        bool groundRight = Physics2D.OverlapCircle(rightDetector.position, groundCheckRadius, stickableSurfaceLayer | slimeLayer);
        bool groundAbove = Physics2D.OverlapCircle(topDetector.position, groundCheckRadius, stickableSurfaceLayer | slimeLayer);

        return groundLeft || groundRight || groundAbove;
    }

    bool isRotatedOnWall()
    {
        float rotation = rb.rotation % 360;
        return ((rotation > 240 && rotation < 300) || (rotation < -60 && rotation > -120) || (rotation > 60 && rotation < 120) || (rotation < -240 && rotation > -300));
    }

    //  Wall Detection Using OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & slimeLayer) != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // cancel upward boost

            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer) && isRotatedOnWall() && Input.GetKey(KeyCode.Space))
                {
                    StickToSlime();
                    break;
                }
            }
        }
        else if (((1 << collision.gameObject.layer) & stickableSurfaceLayer) != 0)
        {
            if (Input.GetKey(KeyCode.Space) &&
                Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer) &&
                isRotatedOnWall())
            {
                if (collision.gameObject.GetComponent<PlatformMovement>() != null)
                {
                    isOnMovingPlatform = true;
                }

                StickToWall(collision.transform);
            }
        }
    }


    // Restick player if pressing Space while already on the wall (Same as OnEnter but for continuous checks)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & slimeLayer) != 0)
        {
            // Checks if player is touching/rotated on wall
            if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer) && isRotatedOnWall())
            {
                if (Input.GetKey(KeyCode.Space) && !isStickingToWall) // if holding space stick to wall
                {
                    StickToSlime();
                }
                else if (isStickingToWall && !TimerOn)
                {
                    // Start the timer if we're stuck but timer isn't running
                    TimerOn = true;
                }
            }
            else if (isOnSlime && !Input.GetKey(KeyCode.Space))
            {
                unstickPlayer();
            }

        }
        else if (((1 << collision.gameObject.layer) & stickableSurfaceLayer) != 0)
        {
            // Checks if player is touching/rotated on wall
            if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer) && isRotatedOnWall())
            {
                if (Input.GetKey(KeyCode.Space) && !isStickingToWall)
                {
                    StickToWall();
                }
                else if (isStickingToWall && !TimerOn)
                {
                    // Start the timer if we're stuck but timer isn't running
                    TimerOn = true;
                }
            }
            else if (isStickingToWall)
            {
                // Lost proper wall contact
                unstickPlayer();
            }
        }
    }



    private void StickToWall(Transform platform = null)
    {
        isStickingToWall = true;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerSticking;
        float rotation = rb.rotation % 360;

        if ((rotation > 240 && rotation < 300) || (rotation < -60 && rotation > -120))
        {
            rb.rotation = 270;
        }

        else if ((rotation > 60 && rotation < 120) || (rotation < -240 && rotation > -300))
        {
            rb.rotation = 90;
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        if (isOnMovingPlatform && platform != null)
        {
            transform.SetParent(platform); // Parent to platform to move with it
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Don't freeze position
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }

        TimerOn = true;
        stickTime = 3f;
    }

    private void StickToSlime()
    {
        isStickingToWall = true;
        isOnSlime = true;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerSticking;
        float rotation = rb.rotation % 360;

        if ((rotation >= 240 && rotation <= 300) || (rotation <= -60 && rotation >= -120)) // on right wall
        {
            rb.rotation = 270;
        }
        else if ((rotation > 60 && rotation < 120) || (rotation < -240 && rotation > -300)) // on left wall
        {
            rb.rotation = 90;
        }

        rb.linearVelocity = new Vector2(0, -1); // Put downward velocity on player
        rb.gravityScale = 0f; // Reduce gravity when on slime
        rb.constraints = RigidbodyConstraints2D.FreezePositionX; // Freeze x position
        TimerOn = true;
        stickTime = 3f;
        Debug.Log("STUCK TO SLIME! GravityScale = " + rb.gravityScale + " | Constraints = " + rb.constraints);

    }

    private void checkOnFloor()
    {
        float rotation = rb.rotation % 360;
        if (((rotation > -15 && rotation < 15) || (rotation > 345 && rotation < 360) || (rotation > -360 && rotation < -345)) && isCurrentlyGrounded && rb.linearVelocity.magnitude < 0.01f) // on ground
        {
            rb.rotation = 0;
            rb.angularVelocity = 0;
        }
    }

    //Resets time to regular
    private void regularSpeedMotion()
    {
        Time.timeScale = normalFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    //Slows down time
    private void slowSpeedMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void unstickPlayer()
    {
        isStickingToWall = false;
        isOnSlime = false;
        isCharging = false; // Resets sprite
        storedLeanAngle = 0f; // Resets stored angle
        rb.gravityScale = 10;
        rb.constraints = RigidbodyConstraints2D.None;

        Vector2 pushDir = Vector2.zero;
        float rotation = rb.rotation % 360;

        if ((rotation > 240 && rotation < 300) || (rotation < -60 && rotation > -120)) // on right wall
        {
            pushDir = new Vector2(-2f, -0.1f).normalized;
        }
        else if ((rotation > 60 && rotation < 120) || (rotation < -240 && rotation > -300)) // on left wall
        {
            pushDir = new Vector2(2f, -0.1f).normalized;
        }

        // Start Coroutine to temporarily disable sticking
        StartCoroutine(TemporarilyDisableStickable());

        rb.AddForce(pushDir * 2f, ForceMode2D.Impulse);
    }

    private IEnumerator TemporarilyDisableStickable()
    {
        if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer))
        {
            if (tilemap != null)
            {
                tilemap.layer = LayerMask.NameToLayer("Default"); // Change to non-stickable
                yield return new WaitForSeconds(0.5f); // Wait 0.5 seconds
                tilemap.layer = LayerMask.NameToLayer("StickableSurface"); // Change back to stickable
            }
        }
        else if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, slimeLayer))
        {
            if (slimeTilemap != null)
            {
                slimeTilemap.layer = LayerMask.NameToLayer("Default"); // Change to non-stickable
                yield return new WaitForSeconds(0.5f); // Wait 0.5 seconds
                slimeTilemap.layer = LayerMask.NameToLayer("Slime"); // Change back to slime
            }
        }
        else
        {
            Debug.LogWarning("Tilemap not found");
        }
    }
}
