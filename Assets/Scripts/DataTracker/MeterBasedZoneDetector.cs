using UnityEngine;

public class MeterBasedZoneDetector : MonoBehaviour
{
    [Header("References")]
    public HeightTracker heightTracker; // We'll call GetCurrentMeters() on this
    public Stopwatch stopwatch;         // We'll call GetFormattedTime() on this

    [Header("Meter-Based Zone Boundaries")]
    // For example: [2, 5, 8, 11, 14, 16, 19]
    // That yields 8 total zones (index 0..7).
    public int[] zoneTopMeters;

    [Header("Optional Zone Names (Should be one more than zoneTopMeters)")]
    // e.g. ["Zone0", "Zone1", "Zone2", "Zone3", "Zone4", "Zone5", "Zone6", "Zone7"]
    public string[] zoneNames;

    private int lastZoneIndex = -1;

    void Update()
    {
        // 1) Get the player's meter count from HeightTracker
        int currentMeters = heightTracker.GetCurrentMeters();

        // 2) Determine which zone index that meter count is in
        int currentZoneIndex = GetZoneIndex(currentMeters);

        // 3) If we changed zones since last frame, log that event
        if (currentZoneIndex != lastZoneIndex)
        {
            lastZoneIndex = currentZoneIndex;

            // If we have enough names, use them; else fallback to "Zone X"
            string zoneLabel;
            if (zoneNames != null && currentZoneIndex < zoneNames.Length)
                zoneLabel = zoneNames[currentZoneIndex];
            else
                zoneLabel = "Zone " + currentZoneIndex;

            // 4) Grab the current stopwatch time in "mm:ss"
            string timeString = (stopwatch != null)
                                ? stopwatch.GetFormattedTime()
                                : Time.time.ToString("F2");

            // 5) Log it with the LocalDataLogger
            LocalDataLogger.Instance.RecordZoneEntry(zoneLabel, timeString);

            Debug.Log($"[MeterBasedZoneDetector] Entered {zoneLabel} at {timeString}");
        }
    }

    // Figures out which zone index the given meter count belongs to
    private int GetZoneIndex(int meters)
    {
        for (int i = 0; i < zoneTopMeters.Length; i++)
        {
            if (meters < zoneTopMeters[i])
                return i;
        }
        // If we exceed all thresholds, we go into the final zone
        return zoneTopMeters.Length;
    }
}
