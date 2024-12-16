using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight2D : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float rotationSpeed = 500f;      // How fast the flashlight rotates
    public Vector3 positionOffset;          // Adjustable offset for flashlight position

    private Light2D flashlight;             // Reference to the Light2D component
    private PlayerMovement playerMovement;

    void Start()
    {
        // Get the Light2D component
        flashlight = GetComponent<Light2D>();

        // Get the PlayerMovement script from the parent
        playerMovement = GetComponentInParent<PlayerMovement>();

        if (flashlight == null)
        {
            Debug.LogError("Light2D component is missing on this GameObject!");
        }
    }

    void Update()
    {
        UpdateFlashlightDirection();
        UpdateFlashlightPosition();
    }

    void UpdateFlashlightDirection()
    {
        if (playerMovement == null) return;

        Vector2 inputDirection = Vector2.zero;

        // Get input direction
        if (playerMovement.playerID == 1)
        {
            inputDirection.x = Input.GetAxisRaw("Horizontal");
            inputDirection.y = Input.GetAxisRaw("Vertical");
        }

        // Only rotate if there’s movement
        if (inputDirection != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;

            // Smoothly rotate the flashlight to the target angle
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0, 0, targetAngle),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void UpdateFlashlightPosition()
    {
        if (playerMovement == null) return;

        // Keep the flashlight at the player's position + offset
        transform.position = playerMovement.transform.position + positionOffset;
    }
}
