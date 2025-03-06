using UnityEngine;
using System.Collections;


public class PlungerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rotationSpeed = 50f;
    public float airRotationSpeed = 100f;
    public float maxPullBack = 45f;
    public float minLaunchForce = 5f;
    public float maxLaunchForce = 20f;
    private float leanAngle = 0f;
    private bool isCharging = false;
    private bool isStickingToWall = false;
    public LayerMask stickableSurfaceLayer;
    public Transform bottomDetector;
    public float groundCheckRadius = 0.1f; // For ground check
    public float wallCheckRadius = 0.1f;   // For wall check
    private bool wasGrounded = false;
    private float storedLeanAngle = 0f;
    public float normalFactor = 1f;
    public float slowdownFactor = 0.7f;
    public bool canStick = false;
    public Sprite PlayerStanding; 
    public Sprite PlayerLeft1;
    public Sprite PlayerLeft2;
    public Sprite PlayerLeft3;
    public Sprite PlayerRight1;
    public Sprite PlayerRight2;
    public Sprite PlayerRight3;
    public bool TimerOn = false;
    public float stickTime;
    public bool isCurrentlyGrounded;
    private float stickCooldown = 0f;
    private float stickCooldownTime = 3f;


    void Awake()
    {
        stickTime = 3f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isCurrentlyGrounded = IsGrounded();


        // Debug print when grounded state changes
        if (isCurrentlyGrounded && !wasGrounded)
        {
            regularSpeedMotion();
            Debug.Log("Player is grounded.");
        }
        else if (!isCurrentlyGrounded && wasGrounded)
        {
            slowSpeedMotion();
            Debug.Log("Player is airborne.");
        }
        wasGrounded = isCurrentlyGrounded;

        if(isCurrentlyGrounded && !isStickingToWall)
        {
            stickCooldown = 0f;
        }

        if(!isCharging && !isStickingToWall)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;
        }

        // Handle movement
        if ((isCurrentlyGrounded || isStickingToWall) && Input.GetKey(KeyCode.Space))
        {
            HandleCharging();
        }
        else if (!isCurrentlyGrounded && !isStickingToWall)
        {
            HandleAirRotation();
        }

        canStick = Input.GetKey(KeyCode.Space);

        if(Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity == Vector2.zero) // Release to launch
        {
            Launch();
        }

        if (isStickingToWall && Input.GetKeyDown(KeyCode.Space))
        {
            StickToWall(); // Stick if pressing Space while on the wall
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
    }

    void HandleCharging()
    {

        rb.linearVelocity = Vector2.zero;
        float input = -Input.GetAxisRaw("Horizontal");
        if (input != 0)
        {
            isCharging = true;
            storedLeanAngle += input * rotationSpeed * Time.deltaTime;
            storedLeanAngle = Mathf.Clamp(storedLeanAngle, -maxPullBack, maxPullBack);

            float lowCharge = maxPullBack * 0.33f;
            float medCharge = maxPullBack * 0.66f;

            if (storedLeanAngle > medCharge)
            { //Max right charge
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
            else if (storedLeanAngle < -medCharge)
            { //Max left charge
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

        this.gameObject.GetComponent<SpriteRenderer>().sprite = PlayerStanding;

        float chargePercent = Mathf.Abs(storedLeanAngle) / maxPullBack;
        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, chargePercent);

        float input = -Input.GetAxisRaw("Horizontal");

        float angleRad = storedLeanAngle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;
        Vector2 upWallLaunchDir, downWallLaunchDir;

        rb.linearVelocity = Vector2.zero; // Reset velocity
        rb.gravityScale = 10; // Reset gravity
        rb.constraints = RigidbodyConstraints2D.None; // Unfreeze movement
        
        if(isStickingToWall)
        {
            float rotation = rb.rotation % 360;

            if((rotation > 255 && rotation < 285) || (rotation < -75 && rotation > -105)) // on right wall
            {
                upWallLaunchDir = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;
                downWallLaunchDir = new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)).normalized;

                if(input < 0) // facing upwards
                    rb.AddForce(upWallLaunchDir * launchForce, ForceMode2D.Impulse);
                else if(input > 0)
                    rb.AddForce(downWallLaunchDir * launchForce, ForceMode2D.Impulse);
            }
            else if((rotation > 75 && rotation < 105) || (rotation < -255 && rotation > -285)) // on left wall
            {
                upWallLaunchDir = new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)).normalized;
                downWallLaunchDir = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;

                if(input < 0) // facing upwards
                    rb.AddForce(upWallLaunchDir * launchForce, ForceMode2D.Impulse);
                else if(input > 0)
                    rb.AddForce(downWallLaunchDir * launchForce, ForceMode2D.Impulse);
            }
        }
        else
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

        // Reset
        isCharging = false;
        isStickingToWall = false;
        storedLeanAngle = 0f;
    }

    //  Ground Detection Using OverlapCircle
    bool IsGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(bottomDetector.position, groundCheckRadius, stickableSurfaceLayer);
        Debug.DrawRay(bottomDetector.position, Vector2.down * groundCheckRadius, hit != null ? Color.green : Color.red);
        return hit != null;
    }

    bool isRotatedOnWall()
    {
        float rotation = Mathf.Abs(rb.rotation % 360);
        return (Mathf.Abs(rotation - 90) <= 15 || Mathf.Abs(rotation - 270) <= 15);
    }

    //  Wall Detection Using OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & stickableSurfaceLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer) && isRotatedOnWall() && Input.GetKey(KeyCode.Space))
                {
                    StickToWall();
                    break;
                }
            }
        }
    }

    // Restick player if pressing Space while already on the wall (Same as OnEnter but for continuous checks)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(stickCooldown > 0f) return;

        if (((1 << collision.gameObject.layer) & stickableSurfaceLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer) && isRotatedOnWall() && Input.GetKey(KeyCode.Space))
                {
                    StickToWall(); 
                    break;
                }
            }
        }
    }

    private void StickToWall()
    {
        if(stickCooldown > 0f) return; // Prevents resticking immediately after stick timer

        isStickingToWall = true;
        rb.linearVelocity = Vector2.zero;

        rb.gravityScale = 0; // Freeze gravity while sticking to wall
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        TimerOn = true;
        stickTime = 3f;
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



    private void unstickPlayer()
    {
        isStickingToWall = false;
        isCharging = false; // Resets sprite
        storedLeanAngle = 0f; // Resets stored angle
        rb.gravityScale = 10;
        rb.constraints = RigidbodyConstraints2D.None;
        stickCooldown = stickCooldownTime; // Initiates cooldown

        Vector2 pushDir = Vector2.zero;
        float rotation = rb.rotation % 360;

        if ((rotation > 255 && rotation < 285) || (rotation < -75 && rotation > -105)) // on right wall
        {
            pushDir = new Vector2(-2f, -0.1f).normalized;
        }
        else if ((rotation > 75 && rotation < 105) || (rotation < -255 && rotation > -285)) // on left wall
        {
            pushDir = new Vector2(2f, -0.1f).normalized;
        }

        rb.AddForce(pushDir * 2f, ForceMode2D.Impulse);

        // Start Coroutine to temporarily disable sticking
        StartCoroutine(TemporarilyDisableStickable());
    }

    private IEnumerator TemporarilyDisableStickable()
    {
        GameObject tilemap = GameObject.Find("Tilemap"); // Adjust this to your actual Tilemap's name

        if (tilemap != null)
        {
            tilemap.layer = LayerMask.NameToLayer("Default"); // Change to non-stickable
            yield return new WaitForSeconds(0.5f); // Wait 0.5 seconds
            tilemap.layer = LayerMask.NameToLayer("StickableSurface"); // Change back to stickable
        }
        else
        {
            Debug.LogWarning("Tilemap not found! Make sure it's named correctly in the hierarchy.");
        }
    }

}