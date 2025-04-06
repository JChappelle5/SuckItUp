using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Rigidbody2D playerRb;
    bool isFalling = false;
    int fallHeight;
    int currHeight;
    float cumulFallChance = 0;
    float timeChance;
    float timeSinceHighScore = 0;
    int maxHeight = 0;
    float saveTracker = 1f;
    float resetSaveTimer = 0f;

    void Update()
    {
        float verticalVelocity = playerRb.linearVelocity.y;
        int meters = Mathf.FloorToInt(playerRb.position.y / 5.235f);

        timeSinceHighScore += Time.deltaTime;

        // START FALL
        if (verticalVelocity < -0.1f && !isFalling)
        {
            isFalling = true;
            fallHeight = meters;
        }

        // END FALL
        if (isFalling && verticalVelocity >= 0f)
        {
            isFalling = false;
            currHeight = meters;
            fallDistance(fallHeight, currHeight);
        }

        if(!isFalling){
            if(maxHeight < currHeight){
                maxHeight = currHeight;
                timeSinceHighScore = 0;
            }

            resetSaveTimer += Time.deltaTime;

            if (resetSaveTimer >= saveTracker)
            {
                SavePosition(transform.position);
                resetSaveTimer = 0f;
            }
        }
    }

    void fallDistance(int start, int end)
    {
        if(start - end < 5) // ignores falls less than 5 meters
        {
            return;
        }
        else
        {
            angelTriggerChance(start-end); //Counts all drops greater than or equal to 5 meters. 
        }
    }

    void angelTriggerChance(int currFall){
        /*The angel is triggered by cumulative fall distance,
          the current fall distance, and how much time as passed without them reaching a new maximum height.
          the cumulative fall distance and time past has less influence overall (12%/12% respectively)
          while the current fall has the greatest impact (40%) which scales exponentially to your fall*/

        int currFallChance = (int) ((currFall * currFall) * 0.05);
        if(currFallChance > 40){ //Caps the chance to 40
            currFallChance = 40;
        }

        int hours = Mathf.FloorToInt(timeSinceHighScore / 3600);
        int minutes = Mathf.FloorToInt((timeSinceHighScore % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeSinceHighScore % 60);

        timeChance = ((minutes * 60) + seconds) / 25; //max is 5 minutes
        if(hours > 0 || minutes > 5){ //caps the chance to 12
            timeChance = 12;
        }

        float totalFallChance = currFallChance + cumulFallChance + timeChance; //sums the chance
        int randomNumGen = Random.Range(10,90); //rolls 10-90 and compare the "chance" to the rng

        Debug.Log(currFallChance + " " + timeChance + " " + cumulFallChance + " " + totalFallChance + " compared to " + randomNumGen);
        if (totalFallChance >= randomNumGen){ //rolls for angel mechanic chance
            cumulFallChance = 0;
            triggerAngel();
        }
        else{
            //If angel mechanic didn't occur, the current fall will be added to the cumulative fall 
            cumulFallChance += (float) ((currFall * currFall) * 0.01);
            if(cumulFallChance > 12){ //Caps the chance to 12
                cumulFallChance = 12;
            }
        }
    }
    void triggerAngel()
    {
        //resets player's position
        float x = PlayerPrefs.GetFloat("posX", 0f); 
        float y = PlayerPrefs.GetFloat("posY", 0f);
        float z = PlayerPrefs.GetFloat("posZ", 0f);
    
        transform.position = new Vector3(x, y, z);
        return;
    }

    void SavePosition(Vector3 position)
    { 
        //save players position
        PlayerPrefs.SetFloat("posX", position.x);
        PlayerPrefs.SetFloat("posY", position.y);
        PlayerPrefs.SetFloat("posZ", position.z);
        PlayerPrefs.Save();
    }
}
