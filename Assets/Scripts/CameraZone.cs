using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector3 cameraPosition; // where the camera should move to when player enters
    public float transitionTime = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered camera zone: " + name);
            CameraController.Instance.MoveToPosition(cameraPosition, transitionTime);
        }
    }

    // Draws the camera target location in the Scene view
    private void OnDrawGizmos()
    {
        Camera mainCam = Camera.main;

        if (mainCam != null && mainCam.orthographic)
        {
            float height = mainCam.orthographicSize * 2f;
            float width = height * mainCam.aspect;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraPosition, new Vector3(width, height, 0f));
        }
    }


}
