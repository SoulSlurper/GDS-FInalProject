using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Tooltip("The destination teleporter this one will send players to")]
    public Teleporter destination;

    [Tooltip("Cooldown time in seconds before teleporter can be used again")]
    public float cooldownTime = 5f;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    // Visual feedback (optional)
    public SpriteRenderer spriteRenderer;
    public Color activeColor = Color.green;
    public Color cooldownColor = Color.red;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        UpdateVisuals();
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                UpdateVisuals();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOnCooldown && other.CompareTag("Player"))
        {
            if (destination != null)
            {
                // Start cooldown on both teleporters to prevent immediate teleport back
                StartCooldown();
                destination.StartCooldown();

                // Play teleport enter sound
                SoundManager.Instance?.PlayTeleportEnterSound();

                // Move player to destination
                other.transform.position = destination.transform.position;

                // Play teleport exit sound at the destination
                SoundManager.Instance?.PlayTeleportExitSound();
            }
            else
            {
                Debug.LogWarning("No destination teleporter assigned!", this);
            }
        }
    }


    public void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isOnCooldown ? cooldownColor : activeColor;
        }
    }
}