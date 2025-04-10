using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void MoveToPosition(Vector3 targetPos, float duration)
    {
        StopAllCoroutines();
        if (duration <= 0f)
        {
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z); // hard snap
        }
        else
        {
            StartCoroutine(SmoothMove(targetPos, duration));
        }
    }

    IEnumerator SmoothMove(Vector3 target, float time)
    {
        Vector3 start = transform.position;
        target.z = start.z; // lock Z position

        float elapsed = 0f;
        while (elapsed < time)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}
