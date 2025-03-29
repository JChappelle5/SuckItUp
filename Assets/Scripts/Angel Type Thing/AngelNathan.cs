using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Rigidbody2D playerRb;
    bool isFalling = false;
    int fallHeight;
    int currHeight;
    int cumulFallChance = 0;
    int timeChance;
    float timeSinceHighScore = 0;
    int maxHeight = 0;
    void Update()
    {
        
        float verticalVelocity = playerRb.linearVelocity.y;
        int meters = Mathf.FloorToInt(playerRb.position.y / 5.235f);

        timeSinceHighScore = Time.deltaTime;

        if(verticalVelocity < 0) //checks for when the player is falling
        {
            fallHeight = meters; //tracks the height they dropped from
            isFalling = true;
        }
        if(isFalling == true && verticalVelocity == 0)
        {
            currHeight = meters; //checks current height after fall
            isFalling = false;
            fallDistance(fallHeight, currHeight);
        }

        if(maxHeight < currHeight){
            maxHeight = currHeight;
            timeSinceHighScore = 0;
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
          the cumulative fall distance and time past has less influence overall (10%/10% respectively)
          while the current fall has the greatest impact (50%) which scales exponentially to your fall*/

        int currFallChance = (int) ((currFall * currFall) * 0.02);
        if(currFallChance > 50){ //Caps the chance to 50
            currFallChance = 50;
        }

        int hours = Mathf.FloorToInt(timeSinceHighScore / 3600);
        int minutes = Mathf.FloorToInt((timeSinceHighScore % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeSinceHighScore % 60);

        timeChance = ((minutes * 60) + seconds) / 25; //max is 5 minutes
        if(hours > 0 || minutes > 5){ //caps the chance to 12
            timeChance = 12;
        }

        int totalFallChance = currFallChance + cumulFallChance + timeChance; //sums the chance
        int randomNumGen = Random.Range(0,101); //rolls 1-100 and compare the "chance" to the rng

        if (totalFallChance < randomNumGen){ //rolls for angel mechanic chance
            Debug.Log("It happened");
            cumulFallChance = 0;
            triggerAngel();
        }
        else{
            //If angel mechanic didn't occur, the current fall will be added to the cumulative fall 
            cumulFallChance += (int) ((currFall * currFall) * 0.005);
            if(cumulFallChance > 12){ //Caps the chance to 12
                cumulFallChance = 12;
            }
        }
    }
    void triggerAngel()
    {
        return;
    }
}
