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

        stuckPosition = hit.ClosestPoint(transform.position);
        transform.position = stuckPosition;

        plungerRb.linearVelocity = Vector2.zero;
        plungerRb.angularVelocity = 0f;
        plungerRb.bodyType = RigidbodyType2D.Kinematic;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.gravityScale = 0;
        playerRb.constraints = RigidbodyConstraints2D.FreezePosition;

        // Attach the plunger to the object it's stuck to
        transform.parent = hit.transform;
    }



    void ReleasePlunger()
    {
        Debug.Log("Plunger Released!");

        isStuck = false;
        stuckSurface = null;

        // Allow plunger movement again
        plungerRb.bodyType = RigidbodyType2D.Dynamic;

        // Unfreeze player movement
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.gravityScale = 1;
    }
}
