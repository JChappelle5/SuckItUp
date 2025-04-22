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
    public bool hasSaved = false;
    public VolumeSettings volumeSettings;

    void Start()
    {
        // Find necessary objects/scripts
        player = GameObject.FindGameObjectWithTag("Player");
        timerScript = stopwatch.GetComponent<Stopwatch>();
        angelScript = player.GetComponent<Angel>();
        narratorScript = narratorAI.GetComponent<PlayerRageEvents>();

        bool firstTimeLaunch = false;

        // Check if save file exists, if not create one
        if(!File.Exists(Application.persistentDataPath + SAVE_FILENAME))
        {
            firstTimeLaunch = true;
            SaveData(spawnPosition, firstTimeLaunch);
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
        
        if(timerScript != null)
            model.timeElapsed = timerScript.timeElapsed; 

        if(angelScript != null)
        {
            model.fallChance = angelScript.cumulFallChance;
            model.highScoreTime = angelScript.timeSinceHighScore;
        }

        if(narratorScript != null)
        {
            model.frustrationLevel = narratorScript.frustrationLevel;
            model.lastHeightChkpnt = narratorScript.lastHeightCheckpoint;
            model.highestHeight = narratorScript.highestHeightReached;
            model.timeNoFall = narratorScript.timeWithoutFall;
            model.curNoFallThresh = narratorScript.currentNoFallThreshold;
            model.consecNewHeight = narratorScript.consecutiveNewHeightCount;
        }

        if(volumeSettings != null)
        {
            model.masterVolume = volumeSettings.masterSlider.value;
            model.musicVolume = volumeSettings.musicSlider.value;
            model.sfxVolume = volumeSettings.sfxSlider.value;
            model.narratorVolume = volumeSettings.narratorSlider.value;
        }

        model.firstPlaythrough = false;

        // Save file
        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + SAVE_FILENAME, json);
        Debug.Log("Game saved to " + Application.persistentDataPath + SAVE_FILENAME);
    }

    // Save data on reset specifically
    public void SaveData(Vector2 position, bool firstTimeLaunch)
    {
        // Prevents saving again if already saved
        if(hasSaved) return; 
        hasSaved = true; 

        // Save default values to save file
        SaveModel model = new SaveModel();
        model.playerPos = position;
        model.playerRotation = 0f;
        model.playerVelocity = Vector2.zero;
        
        if(!File.Exists(Application.persistentDataPath + SAVE_FILENAME))
            model.firstPlaythrough = true;

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
        
        if(!firstTimeLaunch && volumeSettings != null && !model.firstPlaythrough)
        {
            model.masterVolume = volumeSettings.masterSlider.value;
            model.musicVolume = volumeSettings.musicSlider.value;
            model.sfxVolume = volumeSettings.sfxSlider.value;
            model.narratorVolume = volumeSettings.narratorSlider.value;
        }
        else
        {
            model.masterVolume = 1f;
            model.musicVolume = 1f;
            model.sfxVolume = 1f;
            model.narratorVolume = 1f;
        }

        model.firstPlaythrough = false;

        // Save filea
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

        volumeSettings.masterSlider.value = model.masterVolume;
        volumeSettings.musicSlider.value = model.musicVolume;
        volumeSettings.sfxSlider.value = model.sfxVolume;
        volumeSettings.narratorSlider.value = model.narratorVolume;

        volumeSettings.SetMasterVolume(model.masterVolume);
        volumeSettings.SetMusicVolume(model.musicVolume);
        volumeSettings.SetSFXVolume(model.sfxVolume);
        volumeSettings.SetNarratorVolume(model.narratorVolume);

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

    // Settings Values
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float narratorVolume;

    public bool firstPlaythrough;
}