using UnityEngine;
using System.Collections;


public class Angel : MonoBehaviour
{
    public Rigidbody2D playerRb;
    bool isFalling = false;
    int fallHeight;
    int currHeight;
    public float cumulFallChance = 0;
    float timeChance;
    public float timeSinceHighScore = 0;
    int maxHeight = 0;
    float saveTracker = 1f;
    float resetSaveTimer = 0f;
    public GameObject drainSnake;
    public Transform playerTransform;
    public SpriteRenderer snakeRenderer;
    public Sprite[] snakeSprites;
    private int[] swingPattern = { 0, 1, 2, 1, 0, 3, 4, 3, 0 };
    private float swingInterval = 0.1f;
    private Coroutine swingCoroutine;
    public SpriteRenderer playerRenderer;
    public Sprite[] wrappedSprites;
    public float wrapFrameDelay = 0.05f;
    public PlungerMovement spriteControllerScript;
    bool angelTriggeredThisFall = false;
    public Transform currentPlatform;
    public bool isOnMovingPlatform = false;
    public bool canSavePosition = true;
    public bool isCarryingPlayer = false;

    public bool isOnGround;
    public PlungerMovement flingScript;

    void Start()
    {
        playerTransform = transform;
    }


    void Update()
    {
        float verticalVelocity = playerRb.linearVelocity.y;
        int meters = Mathf.FloorToInt(playerRb.position.y / 5.235f);

        timeSinceHighScore += Time.deltaTime;

        // START FALL
        if (verticalVelocity < -0.1f && !isFalling)
        {
            isFalling = true;
            fallHeight = Mathf.FloorToInt(playerRb.position.y / 5.235f);
            angelTriggeredThisFall = false;
        }

        // END FALL
        if (isFalling && verticalVelocity >= 0f)
        {
            isFalling = false;
            currHeight = Mathf.FloorToInt(playerRb.position.y / 5.235f);

            // Optional: only call this if angel didn't already trigger
            if (!angelTriggeredThisFall)
            {
                fallDistance(fallHeight, currHeight);
            }
        }

        if (!isFalling)
        {
            if (maxHeight < currHeight)
            {
                maxHeight = currHeight;
                timeSinceHighScore = 0;
            }

            resetSaveTimer += Time.deltaTime;

            if (resetSaveTimer >= saveTracker && !isOnMovingPlatform && canSavePosition && isOnGround)
            {
                SavePosition(transform.position);
                resetSaveTimer = 0f;
            }
        }

        checkOnFloor();
    }

    void fallDistance(int start, int end)
    {
        if (start - end < 5) // ignores falls less than 5 meters
        {
            return;
        }
        else
        {
            angelTriggerChance(start - end); //Counts all drops greater than or equal to 5 meters. 
        }
    }

    void angelTriggerChance(int currFall)
    {
        /*The angel is triggered by cumulative fall distance,
          the current fall distance, and how much time as passed without them reaching a new maximum height.
          the cumulative fall distance and time past has less influence overall (12%/12% respectively)
          while the current fall has the greatest impact (40%) which scales exponentially to your fall*/

        // THIS IS FOR TESTING PURPOSES
        //triggerAngel();

        int currFallChance = (int)((currFall * currFall) * 0.05);
        if (currFallChance > 40)
        { //Caps the chance to 40
            currFallChance = 40;
        }

        int hours = Mathf.FloorToInt(timeSinceHighScore / 3600);
        int minutes = Mathf.FloorToInt((timeSinceHighScore % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeSinceHighScore % 60);

        timeChance = ((minutes * 60) + seconds) / 25; //max is 5 minutes
        if (hours > 0 || minutes > 5)
        { //caps the chance to 12
            timeChance = 12;
        }

        float totalFallChance = currFallChance + cumulFallChance + timeChance; //sums the chance
        int randomNumGen = Random.Range(1, 100); //rolls 1-100 and compare the "chance" to the rng

        Debug.Log(currFallChance + " " + timeChance + " " + cumulFallChance + " " + totalFallChance + " compared to " + randomNumGen);
        if (totalFallChance >= randomNumGen)
        { //rolls for angel mechanic chance
            cumulFallChance = 0;
            triggerAngel();
        }
        else
        {
            //If angel mechanic didn't occur, the current fall will be added to the cumulative fall 
            cumulFallChance += (float)((currFall * currFall) * 0.04);
            if (cumulFallChance > 12)
            { //Caps the chance to 12
                cumulFallChance = 12;
            }
        }
    }

    void triggerAngel()
    {
        StartCoroutine(SnakePickupSequence());
    }


    void SavePosition(Vector3 position)
    {
        //save players position
        Debug.Log("hi");
        PlayerPrefs.SetFloat("posX", position.x);
        PlayerPrefs.SetFloat("posY", position.y);
        PlayerPrefs.SetFloat("posZ", position.z);
        PlayerPrefs.Save();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the other object has a PlatformMovement script
        if (collision.gameObject.GetComponent<PlatformMovement>() != null)
        {
            isOnMovingPlatform = true;
            currentPlatform = collision.transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlatformMovement>() != null)
        {
            if (collision.transform == currentPlatform)
            {
                currentPlatform = null;
                isOnMovingPlatform = false;

                StartCoroutine(EnableSavingAfterDelay(0.25f));
            }
        }
    }

    private void checkOnFloor()
    {
        float rotation = playerRb.rotation % 360;
        if(flingScript.IsGrounded())
        {
            if (((rotation > -15 && rotation < 15) || (rotation > 345 && rotation < 360) || (rotation > -360 && rotation < -345)) && flingScript.IsGrounded() && playerRb.linearVelocity.magnitude < 0.01f) // on ground
            {
                isOnGround = true;
            }
            else{
                isOnGround = false;
            }
        }
    }

    IEnumerator EnableSavingAfterDelay(float delay)
    {
        canSavePosition = false;
        yield return new WaitForSeconds(delay);
        canSavePosition = true;
    }


    IEnumerator SnakePickupSequence()
    {
        if (drainSnake == null)
        {
            Debug.LogError("Missing drain snake reference!");
            yield break;
        }

        StartSwing();

        float speed = 15f;
        float exitSpeed = speed * 1.5f;

        // Load the saved position
        Vector3 savedPos = new Vector3(
        PlayerPrefs.GetFloat("posX", 0f),
        PlayerPrefs.GetFloat("posY", 0f),
        PlayerPrefs.GetFloat("posZ", 0f)
        );

        // Snake drops down to grab the player 
        drainSnake.SetActive(true);
        spriteControllerScript.enabled = false;

        // Snake comes down
        drainSnake.transform.position = transform.position + new Vector3(0, 18f, 0);

        while (true)
        {
            Vector3 currentGrabPosition = transform.position + new Vector3(0, 1f, 0);

            drainSnake.transform.position = Vector3.MoveTowards(
                drainSnake.transform.position,
                currentGrabPosition,
                speed * Time.deltaTime
            );

            // Stop when snake is close enough to the player's current position
            if (Vector3.Distance(drainSnake.transform.position, currentGrabPosition) < 0.1f)
                break;

            yield return null;
        }
        isCarryingPlayer = true;
        playerTransform.rotation = Quaternion.identity;
        transform.SetParent(drainSnake.transform);
        playerRb.simulated = false;
        StopSwing();
        

        yield return StartCoroutine(PlayWrapUpAnimation());

        // Snake carries player to saved position 
        Vector3 carryPosition = savedPos + new Vector3(0, 1f, 0);
        while (Vector3.Distance(drainSnake.transform.position, carryPosition) > 0.1f)
        {
            drainSnake.transform.position = Vector3.MoveTowards(
                drainSnake.transform.position,
                carryPosition,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // Set player down 
        transform.SetParent(null);
        transform.position = savedPos;
        playerRb.simulated = true;
        playerRb.linearVelocity = Vector2.zero;
        isCarryingPlayer = false;


    yield return StartCoroutine(PlayUnwrapAnimation());

        // Snake exits upward
        float exitDistance = 20f;
        Vector3 exitTarget = drainSnake.transform.position + new Vector3(0, exitDistance, 0);

        while (Vector3.Distance(drainSnake.transform.position, exitTarget) > 0.1f)
        {
            drainSnake.transform.position = Vector3.MoveTowards(
                drainSnake.transform.position,
                exitTarget,
                exitSpeed * Time.deltaTime
            );
            yield return null;
        }

        drainSnake.SetActive(false);

        spriteControllerScript.enabled = true;

    }




    IEnumerator SwingAnimation()
    {
        int index = 0;
        while (true)
        {
            int spriteIndex = swingPattern[index];
            snakeRenderer.sprite = snakeSprites[spriteIndex];

            index = (index + 1) % swingPattern.Length;
            yield return new WaitForSeconds(swingInterval);
        }
    }


    void StartSwing()
    {
        if (swingCoroutine == null)
        {
            swingCoroutine = StartCoroutine(SwingAnimation());
        }
    }

    void StopSwing()
    {
        if (swingCoroutine != null)
        {
            StopCoroutine(swingCoroutine);
            swingCoroutine = null;
        }
    }

    IEnumerator PlayWrapUpAnimation()
    {
        snakeRenderer.sprite = snakeSprites[0];
        for (int i = 0; i < wrappedSprites.Length; i++)
        {
            playerRenderer.sprite = wrappedSprites[i];
            yield return new WaitForSeconds(wrapFrameDelay);
        }
        // Leaves the last wrap frame on screen
        playerRenderer.sprite = wrappedSprites[wrappedSprites.Length - 1];
    }

    IEnumerator PlayUnwrapAnimation()
    {
        for (int i = wrappedSprites.Length - 1; i >= 0; i--)
        {
            playerRenderer.sprite = wrappedSprites[i];
            yield return new WaitForSeconds(wrapFrameDelay);
        }
    }


}