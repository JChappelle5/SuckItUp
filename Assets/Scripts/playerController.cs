using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float BASE_SPEED = 5;
    [SerializeField] private float JUMP_FORCE = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = false; // New variable to track if the player is on the ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ?? Stop movement if the plunger is stuck
        if (FindFirstObjectByType<PlungerStick>().isStuck)
        {
            rb.linearVelocity = Vector2.zero; // Completely stop movement
            return; // Exit the function, preventing movement
        }

        float horizontal = Input.GetAxis("Horizontal");
        Vector3 dir = new Vector3(horizontal, 0, 0);

        rb.linearVelocity = new Vector2(dir.x * BASE_SPEED, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JUMP_FORCE);
            isGrounded = false;
        }
    }


    // ?? Ground Detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Ensure your ground has the "Ground" tag
        {
            isGrounded = true;
        }
    }
}
