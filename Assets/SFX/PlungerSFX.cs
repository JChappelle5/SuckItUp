using UnityEngine;

public class PlungerSFX : MonoBehaviour
{
    public AudioSource audioSource;     // Where the SFX plays
    public AudioClip launchSound;       // Plunger launch SFX

    void Update()
    {
        // Because isCharging is static, qualify it with the class name (PlungerMovement).
        if (Input.GetKeyUp(KeyCode.Space) && PlungerMovement.isCharging)
        {
            if (launchSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(launchSound);
            }
        }
    }
}
