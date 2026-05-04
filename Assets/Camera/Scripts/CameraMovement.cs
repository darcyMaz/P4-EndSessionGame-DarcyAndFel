using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // The target to track with this camera.
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    private Vector3 _velocity;

    // Look Ahead Variables
    [SerializeField] float lookAheadDistance = 0.4f;
    private float currentLookAhead = 0f;
    private float lookAheadVelocity = 0f;
    [SerializeField] float lookAheadSmooth = 0.8f;

    // Min-Max Clamp
    [SerializeField] private float MinY = 0;
    [SerializeField] private float MaxY = 24;
    [SerializeField] private float MinX = -20;
    [SerializeField] private float MaxX = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        // Where is target facing?
        float direction = Mathf.Sign(transform.position.x);

        // Look ahead to where the player is looking.
        float lookAheadTarget = direction * lookAheadDistance;
        currentLookAhead = Mathf.SmoothDamp(currentLookAhead, lookAheadTarget, ref lookAheadVelocity, lookAheadSmooth);

        // Set the camera's target.
        Vector3 desired = target.position + offset;
        desired.x += currentLookAhead;

        // Smoothdamp the position of the camera towards the desired target.
        Vector3 NewPosition = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

        // Clamp that position
        NewPosition.y = Mathf.Clamp(NewPosition.y,MinY,MaxY);
        NewPosition.x = Mathf.Clamp(NewPosition.x,MinX,MaxX);

        transform.position = NewPosition;
    }
}
