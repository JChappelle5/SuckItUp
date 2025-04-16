using UnityEngine;

public class CameraZoneTriggerProxy : MonoBehaviour
{
    public Angel drainSnake; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!drainSnake.isCarryingPlayer) return;

        CameraZone zone = other.GetComponent<CameraZone>();
        if (zone != null)
        {
            Debug.Log("Drain snake triggered camera zone: " + zone.name);
            CameraController.Instance.MoveToPosition(zone.cameraPosition, zone.transitionTime);
        }
    }
}
