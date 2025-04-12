using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    public const string SAVE_FILENAME = "/savefile.json";
    public Rigidbody2D rb;
    public Vector2 spawnPosition = new Vector2(-21.908f, -4.362f); // Default spawn position
    public GameObject stopwatch;
    private Stopwatch timerScript; 
    private GameObject player;
    private Angel angelScript; 
    public GameObject narratorAI;
    private PlayerRageEvents narratorScript;
    private bool isQuitting = false;
    private bool hasSaved = false;

    void Start()
    {
        // Find necessary objects/scripts
        player = GameObject.FindGameObjectWithTag("Player");
        timerScript = stopwatch.GetComponent<Stopwatch>();
        angelScript = player.GetComponent<Angel>();
        narratorScript = narratorAI.GetComponent<PlayerRageEvents>();

        // Check if save file exists, if not create one
        if(!File.Exists(Application.persistentDataPath + SAVE_FILENAME))
        {
            SaveData(spawnPosition);
            LoadData();
        }
        else
        {
            LoadData(); // Load existing save data
        }

        StartCoroutine(AutoSave()); // Start auto-saving
    }

    // Regular player save
    public void SaveData()
    {
        // Prevents saving again if already saved
        if(hasSaved) return; 
        hasSaved = true; 
        
        // Save all current values to save file
        SaveModel model = new SaveModel();
        model.playerPos = rb.position;
        model.playerRotation = rb.rotation;
        model.playerVelocity = rb.linearVelocity;
        
        model.timeElapsed = timerScript.timeElapsed; 

        model.fallChance = angelScript.cumulFallChance;
        model.highScoreTime = angelScript.timeSinceHighScore;

        if(narratorScript != null)
        {
            model.frustrationLevel = narratorScript.frustrationLevel;
            model.lastHeightChkpnt = narratorScript.lastHeightCheckpoint;
            model.highestHeight = narratorScript.highestHeightReached;
            model.timeNoFall = narratorScript.timeWithoutFall;
            model.curNoFallThresh = narratorScript.currentNoFallThreshold;
            model.consecNewHeight = narratorScript.consecutiveNewHeightCount;
        }

        // Save file
        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + SAVE_FILENAME, json);
        Debug.Log("Game saved to " + Application.persistentDataPath + SAVE_FILENAME);
    }

    // Save data on reset specifically
    public void SaveData(Vector2 position)
    {
        // Prevents saving again if already saved
        if(hasSaved) return; 
        hasSaved = true; 

        // Save default values to save file
        SaveModel model = new SaveModel();
        model.playerPos = position;
        model.playerRotation = 0f;
        model.playerVelocity = Vector2.zero;

        // Default time (0)
        model.timeElapsed = 0f;

        // Default angel values
        model.fallChance = 0f;
        model.highScoreTime = 0f;

        // Default narrator values
        if(narratorScript != null)
        {
            model.frustrationLevel = 8f;
            model.lastHeightChkpnt = -4.362f;
            model.highestHeight = -4.362f;
            model.timeNoFall = 0f;
            model.curNoFallThresh = 0f;
            model.consecNewHeight = 0;
        }

        // Save file
        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + SAVE_FILENAME, json);
        Debug.Log("Game saved to " + Application.persistentDataPath + SAVE_FILENAME);
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
        if(rb != null)
        {
            SaveData();
        }
    }

    // Forces save if game crashes/closes forcefully
    void OnDestroy()
    {
        if (!isQuitting && !hasSaved) 
        {
            if(rb != null)
                SaveData(); // Save data when the game is closed
        }
    }

    void LoadData()
    {
        // Loads current values from save file
        SaveModel model = JsonUtility.FromJson<SaveModel>(File.ReadAllText(Application.persistentDataPath + SAVE_FILENAME));
        rb.position = model.playerPos;
        rb.rotation = model.playerRotation;
        rb.linearVelocity = model.playerVelocity;

        // Actual time for timer
        timerScript.timeElapsed = model.timeElapsed;

        angelScript.cumulFallChance = model.fallChance;
        angelScript.timeSinceHighScore = model.highScoreTime;

        if(narratorScript != null)
        {
            narratorScript.frustrationLevel = model.frustrationLevel;
            narratorScript.lastHeightCheckpoint = model.lastHeightChkpnt;
            narratorScript.highestHeightReached = model.highestHeight;
            narratorScript.timeWithoutFall = model.timeNoFall;
            narratorScript.currentNoFallThreshold = model.curNoFallThresh;
            narratorScript.consecutiveNewHeightCount = model.consecNewHeight;
        }

        Debug.Log("Game loaded from " + Application.persistentDataPath + SAVE_FILENAME);
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // every 30 seconds
            SaveData();
            hasSaved = false; // Reset hasSaved to allow for another save
        }
    }
}

public class SaveModel
{
    // Player values
    public Vector2 playerPos;
    public Vector2 playerVelocity;
    public float playerRotation;

    // Timer value
    public float timeElapsed;

    // Angel values
    public float fallChance;
    public float highScoreTime;

    // Narrator values
    public float frustrationLevel;
    public float lastHeightChkpnt;
    public float highestHeight;
    public float timeNoFall;
    public float curNoFallThresh;
    public int consecNewHeight;
}