using UnityEngine;
using System.Collections;

public class WindZone : MonoBehaviour
{
    public float windForce = 8f;
    public Vector2 windDirection = new Vector2(1f, 0.25f);
    public float range = 3f;

    public bool isActive = false;        
    public bool IsBlowing => isBlowing;
    private bool isBlowing = false;

    [Header("Gust Timing")]
    public float minBlowDuration = 2f;
    public float maxBlowDuration = 4f;
    public float minPauseDuration = 2f;
    public float maxPauseDuration = 4f;

    private Rigidbody2D playerRb;
    private Transform player;
    private PlungerMovement movement;
    private PlayerRageEvents rage;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            movement = player.GetComponent<PlungerMovement>();
            rage = player.GetComponent<PlayerRageEvents>();
        }

        StartCoroutine(WindCycleRoutine());
    }

    void FixedUpdate()
    {
        if (!isActive || !isBlowing || player == null || movement == null) return;

        bool isAirborne = !movement.isCurrentlyGrounded && !movement.isStickingToWall;

        if (isAirborne && PlayerInRange())
        {
            Vector2 direction = windDirection.normalized;
            playerRb.AddForce(direction * windForce, ForceMode2D.Force);
        }
    }

    private bool PlayerInRange()
    {
        return Mathf.Abs(player.position.y - transform.position.y) < range;
    }

    private IEnumerator WindCycleRoutine()
    {
        while (true)
        {
            if (isActive)
            {
                isBlowing = true;
                float blowTime = Random.Range(minBlowDuration, maxBlowDuration);
                yield return new WaitForSeconds(blowTime);

                isBlowing = false;
                float pauseTime = Random.Range(minPauseDuration, maxPauseDuration);
                yield return new WaitForSeconds(pauseTime);
            }
            else
            {
                isBlowing = false;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Draw a wire circle around the zone to show its range
        Gizmos.DrawWireSphere(transform.position, range);

        // Draw a direction line to show where wind will push
        Vector3 direction = (Vector3)(windDirection.normalized * 2f);
        Gizmos.DrawLine(transform.position, transform.position + direction);
        Gizmos.DrawSphere(transform.position + direction, 0.1f); // arrow tip
    }
}
