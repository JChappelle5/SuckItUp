using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class LocalDataLogger : MonoBehaviour
{
    public static LocalDataLogger Instance;

    // === FALL STATS ===
    private int totalFalls = 0;
    private float totalFallDistance = 0f;
    private float largestFallDistance = 0f;

    // === ZONE ENTRIES ===
    // We'll store them as a list of "Zone0=01:38" strings
    private List<string> zoneDataList = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //================ FALL RECORDING =================
    // Call this (e.g. from your FallTracker) AFTER you convert from Unity units to "meters"
    public void RecordFall(float distanceFallenMeters)
    {
        totalFalls++;
        totalFallDistance += distanceFallenMeters;
        if (distanceFallenMeters > largestFallDistance)
            largestFallDistance = distanceFallenMeters;

        Debug.Log($"[Logger] Fall recorded: {distanceFallenMeters:F2}m (largest so far: {largestFallDistance:F2}m)");
    }

    //================ ZONE RECORDING =================
    // Called by MeterBasedZoneDetector when we cross into a new zone
    public void RecordZoneEntry(string zoneName, string timeString)
    {
        // e.g. "Zone0=01:38"
        string entry = $"{zoneName}={timeString}";
        zoneDataList.Add(entry);

        Debug.Log($"[Logger] Zone entry recorded: {entry}");
    }

    //================ SAVE TO CSV =================
    public void SaveData()
    {
        // 1) Build the CSV path
        string path = Path.Combine(Application.persistentDataPath, "playtest_log.csv");

        // 2) If it doesn't exist, write a header
        if (!File.Exists(path))
        {
            string header = "DateTime,TotalFalls,TotalFallDistance,LargestFallDistance,ZoneData\n";
            File.WriteAllText(path, header);
        }

        // 3) Construct the zone data string: "Zone0=01:38|Zone1=02:45|..."
        string zoneDataStr = "";
        foreach (var z in zoneDataList)
        {
            zoneDataStr += z + "|";
        }

        // 4) Create a line with your stats
        string newLine = string.Format(
            "{0},{1},{2:F2},{3:F2},\"{4}\"\n",
            DateTime.Now,
            totalFalls,
            totalFallDistance,
            largestFallDistance,
            zoneDataStr
        );

        // 5) Append to file
        File.AppendAllText(path, newLine);

        Debug.Log($"[Logger] Data saved to {path}");
    }

    // Auto-save when the game quits
    private void OnApplicationQuit()
    {
        SaveData();
    }
}
