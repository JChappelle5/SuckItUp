using UnityEngine;

public class PlungerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rotationSpeed = 50f;
    public float airRotationSpeed = 100f;
    public float maxPullBack = 45f;
    public float minLaunchForce = 5;
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

        // Handle movement
        if ((isCurrentlyGrounded || isStickingToWall) && Input.GetKey(KeyCode.Space))
        {
            HandleCharging();
        }
        else if (!isCurrentlyGrounded && !isStickingToWall)
        {
            HandleAirRotation();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            canStick = true;
        }
        else{
            canStick = false;
        }

        if(Input.GetKeyUp(KeyCode.Space)) // Release to launch
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
    }

    void HandleCharging()
    {
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
                if (Physics2D.OverlapCircle(bottomDetector.position, wallCheckRadius, stickableSurfaceLayer) && isRotatedOnWall() && canStick == true)
                {
                    StickToWall();
                    break;
                }
            }
        }
    }

    private void StickToWall()
    {
        
        isStickingToWall = true;
        rb.linearVelocity = Vector2.zero;

        rb.gravityScale = 0; // Freeze gravity while sticking to wall
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        TimerOn = true;
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
        rb.gravityScale = 10;
        rb.constraints = RigidbodyConstraints2D.None;
    }  
}