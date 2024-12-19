using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door State")]
    public bool isOpen = false;

    [Header("Components")]
    private BoxCollider2D solidCollider;  
    private SpriteRenderer spriteRenderer;
    public GameObject interactionText; // Text GameObject to show "Space to use Lockpick"

    [Header("Sprites")]
    public Sprite closedDoorSprite;
    public Sprite openDoorSprite;

    [Header("Text Position")]
    public Vector3 textOffset = new Vector3(0, 1, 0);

    void Start()
    {
        solidCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup text position and hide it initially
        if (interactionText != null)
        {
            interactionText.SetActive(false);
            interactionText.transform.position = transform.position + textOffset;
        }

        UpdateDoorState();
    }

    public void ShowInteractionText()
    {
        if (interactionText != null && !isOpen)
        {
            interactionText.SetActive(true);
        }
    }

    public void HideInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        UpdateDoorState();

        if (isOpen && interactionText != null)
        {
            interactionText.SetActive(false); // Hide text when door opens
        }
    }

    void UpdateDoorState()
    {
        if (isOpen)
        {
            solidCollider.isTrigger = true;
            spriteRenderer.sprite = openDoorSprite;
        }
        else
        {
            solidCollider.isTrigger = false;
            spriteRenderer.sprite = closedDoorSprite;
        }
    }
}
