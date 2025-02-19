using UnityEngine;

public class PlungerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rotationSpeed = 50f; // Speed of leaning
    public float maxPullBack = 45f; // Max lean angle
    public float minLaunchForce = 5f; // Minimum launch power
    public float maxLaunchForce = 20f; // Maximum launch power
    private float leanAngle = 0f;
    private bool isCharging = false; // Detect if pulling back
    public float normalFactor = 1f;
    public float slowdownFactor = 0.75f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleLeaning();

        if (Input.GetKeyUp(KeyCode.Space)) // Release to launch
        {
            regularSpeedMotion();
            Launch();
        }
    }

    void regularSpeedMotion()
    {
        Time.timeScale = normalFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    void slowSpeedMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    void HandleLeaning()
    {
        if (Input.GetKey(KeyCode.Space) && GetComponent<Rigidbody2D>().linearVelocity.y == 0)
        {
            float input = -Input.GetAxisRaw("Horizontal"); // A = Left, D = Right
            if (input != 0)
            {
                isCharging = true;
                leanAngle += input * rotationSpeed * Time.deltaTime;
                leanAngle = Mathf.Clamp(leanAngle, -maxPullBack, maxPullBack);
                rb.MoveRotation(leanAngle);
            }
        }
    }

    void Launch()
    {
        if (!isCharging) return;
        slowSpeedMotion();

        float chargePercent = Mathf.Abs(leanAngle) / maxPullBack; // How much power is stored
        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, chargePercent); // Scale force

        float angleRad = leanAngle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

        // Reset
        isCharging = false;
        leanAngle = 0f;
        rb.MoveRotation(leanAngle);
    }
}