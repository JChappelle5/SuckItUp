using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool hasHit = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(EnableColliderDelayed());

        Destroy(gameObject, lifetime);
    }

    IEnumerator EnableColliderDelayed()
    {
        yield return new WaitForSeconds(0.1f); // tweak as needed
        GetComponent<Collider2D>().enabled = true;
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>(); 

        rb.linearVelocity = direction.normalized * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasHit && other.CompareTag("Player"))
        {
            hasHit = true;

            // Knockback
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            knockbackDirection = new Vector2(knockbackDirection.x, 0).normalized; // only horizontal so they can get hit from above

            float knockbackForce = 10f; // tweak this as needed

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero; // cancel current velocity
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }



            PlungerMovement pm = other.GetComponent<PlungerMovement>();
            GameObject tilemap = GameObject.Find("Tilemap");
            if (pm != null)
            {
                // Only unstick them if they're currently on a wall
                if (pm.isStickingToWall)
                {
                    pm.unstickPlayer();
                }
                else
                {
                    if (tilemap != null)
                    {
                        StartCoroutine(TemporarilyDisableStickableSurface(tilemap));
                    }
                }
            }




            // Explosion
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            GetComponent<Collider2D>().enabled = false;
            anim.SetTrigger("Hit");
            Destroy(gameObject, 0.4f);
        }

    }

    IEnumerator TemporarilyDisableStickableSurface(GameObject tilemap)
    {
        tilemap.layer = LayerMask.NameToLayer("Default"); // Prevent sticking
        yield return new WaitForSecondsRealtime(0.5f);
        tilemap.layer = LayerMask.NameToLayer("StickableSurface"); // Restore
    }


}
