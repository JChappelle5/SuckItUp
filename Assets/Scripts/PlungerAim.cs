using UnityEngine;

public class PlungerAim : MonoBehaviour
{
    public Transform pivotPoint; //  Make sure it's a Transform
    public float rotationSpeed = 10f;

    void Update()
    {
        if (pivotPoint == null) return; // Prevent errors if it's not assigned

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - pivotPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        pivotPoint.rotation = Quaternion.RotateTowards(pivotPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
