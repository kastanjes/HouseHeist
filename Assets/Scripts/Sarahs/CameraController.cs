using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float m_ScreenEdgeBuffer = 4f;           // Space between the targets and the screen edge.
    public float m_MinSize = 6.5f;                  // Minimum orthographic size of the camera.
    public Transform[] m_Targets;                   // All the targets the camera needs to encompass.

    private Camera m_Camera;                        // Reference to the camera.
    private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 m_DesiredPosition;              // The position the camera is moving towards.

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        // Move the camera towards a desired position.
        Move();

        // Adjust the camera's size to encompass all targets.
        Zoom();
    }

    private void Move()
    {
        // Find the average position of the targets.
        FindAveragePosition();

        // Smoothly transition to the desired position.
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // Calculate the average position of all active targets.
        foreach (Transform target in m_Targets)
        {
            if (target == null || !target.gameObject.activeSelf)
                continue;

            averagePos += target.position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        // Maintain the camera's current z-axis position.
        averagePos.z = transform.position.z;

        m_DesiredPosition = averagePos;
    }

    private void Zoom()
    {
        // Calculate the required orthographic size to fit all targets.
        float requiredSize = FindRequiredSize();

        // Smoothly adjust the camera's orthographic size.
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize()
    {
        // Transform the camera's desired position into local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        // Calculate the required size to fit all targets.
        foreach (Transform target in m_Targets)
        {
            if (target == null || !target.gameObject.activeSelf)
                continue;

            // Transform the target position into local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(target.position);

            // Determine the position of the target relative to the desired position.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest size based on the y-axis and x-axis (adjusted for aspect ratio).
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }

        // Add the screen edge buffer to the size.
        size += m_ScreenEdgeBuffer;

        // Ensure the size isn't below the minimum size.
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    public void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_DesiredPosition;

        // Set the camera's orthographic size to fit all targets.
        m_Camera.orthographicSize = FindRequiredSize();
    }
}
