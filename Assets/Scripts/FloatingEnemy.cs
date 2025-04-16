using UnityEngine;

using System.Collections;

public class FloatingEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform[] patrolPoints; 
    public GameObject projectilePrefab; 
    public Transform firePoint; 
    public float attackCooldown = 3f;

    private int currentPoint = 0;
    private float attackTimer = 0f;
    private Animator anim;
    private Transform player;
    private bool isActive = false;

    public float attackRange = 10f;


    /// <summary>
    /// //////////////////////////////////////////////
    /// </summary>


    [Header("Wander Area")]
    public Vector2 areaCenter;
    public float areaRadius = 5f;
    private enum EnemyState { Wandering, Chasing }
    private EnemyState currentState = EnemyState.Wandering;
    private Vector2 wanderTarget;

    IEnumerator Wander()
    {
        while (currentState == EnemyState.Wandering)
        {
            wanderTarget = areaCenter + Random.insideUnitCircle * areaRadius;
            yield return new WaitForSeconds(2f);
        }
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        isActive = true;

        StartCoroutine(Wander());
    }

    void Update()
    {
        if (!isActive) return;
        
        if (currentState == EnemyState.Wandering)
        {
            MoveTowards(wanderTarget);
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceFromCenter = Vector2.Distance(player.position, areaCenter);

        if (distanceFromCenter < areaRadius)
        {
            currentState = EnemyState.Chasing;
        }
        else if (currentState == EnemyState.Chasing && distanceFromCenter >= areaRadius)
        {
            currentState = EnemyState.Wandering;
            StartCoroutine(Wander());
        }
        if (currentState == EnemyState.Chasing)
        {
            // Constrain chase target inside area
            Vector2 directionToPlayer = ((Vector2)player.position - areaCenter).normalized;
            float clampedDistance = Mathf.Min(distanceToPlayer, areaRadius);
            Vector2 chaseTarget = areaCenter + directionToPlayer * clampedDistance;

            MoveTowards(chaseTarget);
        }
        if (currentState == EnemyState.Wandering)
        {
            if (Vector2.Distance(transform.position, wanderTarget) < 0.2f)
            {
                wanderTarget = areaCenter + Random.insideUnitCircle * areaRadius;
            }

            MoveTowards(wanderTarget);
            anim.SetBool("isMoving", (Vector2.Distance(transform.position, wanderTarget) > 0.05f));

        }


        HandleAttack();
        FacePlayer();
    }


    private void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }


    void HandleAttack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Don't attack if player is out of range
        if (distanceToPlayer > attackRange) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            anim.SetTrigger("attack");
            attackTimer = attackCooldown;
        }
    }


    // Called from Animation Event at the right moment during the attack animation
    public void FireProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        proj.GetComponent<EnemyProjectile>().Launch(direction);
    }


    // Call this from your DifficultyManager or wherever you track progress
    public void SetActive(bool value)
    {
        isActive = value;
    }


    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x); // Face right (flip)
        else
            scale.x = Mathf.Abs(scale.x);  // Face left (default)

        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(areaCenter, areaRadius); // wander area

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // attack range
    }


}


/// <summary>
/// /////////////////////////////////////
/// </summary>

//    void Start()
//    {
//        anim = GetComponent<Animator>();
//        player = GameObject.FindWithTag("Player").transform; 
//        isActive = true;
//    }

//    void Update()
//    {
//        if (!isActive) return;

//        Move();
//        HandleAttack();
//        FacePlayer();
//    }

//    void Move()
//    {
//        anim.SetBool("isMoving", true);

//        Transform target = patrolPoints[currentPoint];
//        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

//        if (Vector2.Distance(transform.position, target.position) < 0.1f)
//        {
//            currentPoint = (currentPoint + 1) % patrolPoints.Length;
//        }
//    }

//    void HandleAttack()
//    {
//        if (player == null) return;

//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        // Don't attack if player is out of range
//        if (distanceToPlayer > attackRange) return;

//        attackTimer -= Time.deltaTime;

//        if (attackTimer <= 0f)
//        {
//            anim.SetTrigger("attack");
//            attackTimer = attackCooldown;
//        }
//    }


//    // Called from Animation Event at the right moment during the attack animation
//    public void FireProjectile()
//    {
//        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
//        Vector2 direction = (player.position - firePoint.position).normalized;
//        proj.GetComponent<EnemyProjectile>().Launch(direction);
//    }


//    // Call this from your DifficultyManager or wherever you track progress
//    public void SetActive(bool value)
//    {
//        isActive = value;
//    }


//    void FacePlayer()
//    {
//        if (player == null) return;

//        Vector3 scale = transform.localScale;

//        if (player.position.x > transform.position.x)
//            scale.x = -Mathf.Abs(scale.x); // Face right (flip)
//        else
//            scale.x = Mathf.Abs(scale.x);  // Face left (default)

//        transform.localScale = scale;
//    }

//    void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }

//}
