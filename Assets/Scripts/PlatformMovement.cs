using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;
    private GameObject player; // Store reference to the player

    void Start()
    {
        nextPosition = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject; // Store reference
            player.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player left the platform, resetting parent.");
            player = collision.gameObject; // Ensure reference is set
            Invoke(nameof(ResetPlayerParent), 0.01f); // Small delay to fix hierarchy issues
        }
    }

    void ResetPlayerParent()
    {
        if (player != null && player.transform.parent == transform)
        {
            player.transform.SetParent(null);
            player = null; // Clear reference
        }
    }
}
