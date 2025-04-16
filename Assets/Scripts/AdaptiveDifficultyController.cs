using UnityEngine;

public class AdaptiveDifficultyController : MonoBehaviour
{
    public PlayerRageEvents rageEvents;
    public FloatingEnemy floatingEnemy;
    public WindZone[] windZones;

    private bool hardModeActivated = false;

    void Start()
    {
        foreach (WindZone wz in windZones)
            wz.isActive = false;

    }

    void Update()
    {

        if (rageEvents.IsPlayerDoingTooGood())
        {
            if (!hardModeActivated)
            {
                Debug.Log("<color=red>[DIFFICULTY] Activating wind and enemy (player is doing TOO GOOD)</color>");
                //floatingEnemy.SetActive(true);
                foreach (WindZone wz in windZones)
                    wz.isActive = true;

                hardModeActivated = true;
            }
        }
        else
        {
            if (hardModeActivated)
            {
                Debug.Log("<color=green>[DIFFICULTY] Deactivating wind and enemy (player is struggling again)</color>");
                //floatingEnemy.SetActive(false);
                foreach (WindZone wz in windZones)
                    wz.isActive = false;

                hardModeActivated = false;
            }
        }
    }

}

