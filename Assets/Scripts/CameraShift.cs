using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShift : MonoBehaviour
{
    public Transform player;
    private float screenTop;
    private float screenBottom;
    private float cameraHeight;

    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2f - 1; // total camera height
    }

    void Update()
    {
        screenTop = Camera.main.transform.position.y + Camera.main.orthographicSize; // top of camera
        screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize; // bottom of camera

        if (player.position.y > screenTop)
        {
            Camera.main.transform.position += new Vector3(0, cameraHeight, 0);
        }

        if (player.position.y < screenBottom)
        {
            Camera.main.transform.position -= new Vector3(0, cameraHeight, 0);
        }
    }
}