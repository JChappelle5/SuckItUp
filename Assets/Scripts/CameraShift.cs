//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class CameraShift : MonoBehaviour
//{
//    public Transform player;
//    private float screenTop;
//    private float screenBottom;
//    private float cameraHeight;

//    void Start()
//    {
//        cameraHeight = Camera.main.orthographicSize * 2f - 1; // total camera height
//    }

//    void Update()
//    {
//        screenTop = Camera.main.transform.position.y + Camera.main.orthographicSize; // top of camera
//        screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize; // bottom of camera

//        if (player.position.y > screenTop)
//        {
//            Camera.main.transform.position += new Vector3(0, cameraHeight, 0);
//        }

//        if (player.position.y < screenBottom)
//        {
//            Camera.main.transform.position -= new Vector3(0, cameraHeight, 0);
//        }
//    }
//}

using UnityEngine;
using System.Collections;

public class CameraShift : MonoBehaviour
{
    public Transform player;
    private float screenTop, screenBottom, screenLeft, screenRight;
    private float cameraHeight, cameraWidth;

    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2f - 1; // Total camera height
        cameraWidth = cameraHeight * Camera.main.aspect; // Total camera width
    }

    void Update()
    {
        screenTop = Camera.main.transform.position.y + Camera.main.orthographicSize;
        screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        screenRight = Camera.main.transform.position.x + cameraWidth / 2;
        screenLeft = Camera.main.transform.position.x - cameraWidth / 2;

        // Move camera up
        if (player.position.y > screenTop)
        {
            Camera.main.transform.position += new Vector3(0, cameraHeight, 0);
        }

        // Move camera down
        if (player.position.y < screenBottom)
        {
            Camera.main.transform.position -= new Vector3(0, cameraHeight, 0);
        }

        // Move camera right
        if (player.position.x > screenRight)
        {
            Camera.main.transform.position += new Vector3(cameraWidth, 0, 0);
        }

        // Move camera left
        if (player.position.x < screenLeft)
        {
            Camera.main.transform.position -= new Vector3(cameraWidth, 0, 0);
        }
    }
}
