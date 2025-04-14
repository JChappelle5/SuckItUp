using UnityEngine;

public class WindZoneManager : MonoBehaviour
{
    public WindZone[] windZones;
    public PlayerRageEvents rage;
    public float frustrationThreshold = 3f;  // Frustration level below this = skilled
    public int consecutiveClimbsThreshold = 3;

    void Update()
    {
        bool skilledPlayer = rage.FrustrationLevel <= frustrationThreshold
                          || rage.skillTestPassed
                          || rage.consecutiveNewHeightCount >= consecutiveClimbsThreshold;

        foreach (var zone in windZones)
        {
            zone.isActive = skilledPlayer;
        }
        
        // FOR TESTING PURPOSES
        //foreach (var zone in windZones)
        //{
        //    if (zone != null)
        //        zone.isActive = true; // <--- FORCE ENABLED
        //}


    }
}
