using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerOne; // Reference til Player One's transform
    public Transform playerTwo; // Reference til Player Two's transform
    public float zoomSpeed = 10f; // Hastighed for zoom
    public float minZoom = 5f; // Minimum zoom
    public float maxZoom = 20f; // Maksimum zoom
    public float zoomLimiter = 10f; // Begrænsning af zoom
    public float smoothSpeed = 0.125f; // Hvor glat kameraet skal bevæge sig

    private Vector3 velocity; // Til glat overgang i kameraets position

    void LateUpdate()
    {
        if (playerOne == null || playerTwo == null)
            return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        // Find midtpunktet mellem de to spillere
        Vector3 midPoint = (playerOne.position + playerTwo.position) / 2f;

        // Juster kameraets position mod midtpunktet
        Vector3 newPosition = new Vector3(midPoint.x, midPoint.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothSpeed);
    }

    void ZoomCamera()
    {
        // Find afstanden mellem spillerne
        float distance = Vector3.Distance(playerOne.position, playerTwo.position);

        // Beregn kameraets ønskede zoomniveau baseret på afstanden
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);

        // Juster kameraets orthographic size
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newZoom, Time.deltaTime * zoomSpeed);
    }
}
