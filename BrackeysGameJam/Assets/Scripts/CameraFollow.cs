using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Player

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 6, -12);
    public float smoothSpeed = 5f;
    public bool followRotation = false; // toggle if you want rotation matching

    void LateUpdate()
    {
        if (!target) return;

        // Match player's X + Z but keep offset
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            target.position.z + offset.z
        );

        // Smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Look at the player
        if (followRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, smoothSpeed * Time.deltaTime);
        else
            transform.LookAt(target.position + Vector3.up * 1.5f); // looks at player’s chest/head
    }
}
