using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip walkSound; // Walking sound clip
    public AudioClip jumpSound; // Jump sound clip
    private AudioSource audioSource; // Audio source for playing sounds
    private GameObject player; // Reference to the player GameObject
    private bool isWalking = false; // To check if the player is walking
    private bool isGrounded = true; // To check if the player is grounded
    private bool hasJumped = false; // To ensure the jump sound plays only once
    private float targetVolume = 0.5f; // Reduced volume (half of 1)
    private float maxPlayerSpeed = 5f; // Max speed of the player for volume scaling
    private float speedThreshold = 0.1f; // Speed threshold for movement detection
    private Rigidbody2D rb; // Reference to the Rigidbody2D of the player

    private void Start()
    {
        // Find the player prefab in the scene (assuming it has the "Player" tag)
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No object with the 'Player' tag found in the scene.");
            return;
        }

        // Get the AudioSource component on the player prefab
        audioSource = player.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on the player prefab.");
            return;
        }

        // Get the Rigidbody2D component for velocity checking
        rb = player.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found on the player prefab.");
            return;
        }
    }

    private void Update()
    {
        if (player != null && audioSource != null && rb != null)
        {
            // Check if the player is moving using their velocity (for smoother detection)
            float playerSpeed = rb.velocity.magnitude; // Get the speed of the player

            // Handle walking sound based on player's movement
            if (playerSpeed > speedThreshold) // Consider the player is moving if their speed is above the threshold
            {
                if (!isWalking && isGrounded) // Only play walk sound if grounded
                {
                    isWalking = true;
                    PlayWalkSound();
                }
                else
                {
                    // Adjust sound volume based on player speed
                    audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume * Mathf.Clamp01(playerSpeed / maxPlayerSpeed), Time.deltaTime * 5f);
                }
            }
            else
            {
                // Stop the walking sound when the player stops moving
                if (isWalking)
                {
                    isWalking = false;
                    StopWalkSound();
                }
            }

            // Check if the player is jumping and is grounded
            if (Input.GetButtonDown("Jump") && isGrounded && !hasJumped) // Ensure the jump sound plays only once per jump
            {
                isGrounded = false; // Player is no longer grounded after jumping
                hasJumped = true; // Mark that the player has jumped
                StopWalkSound(); // Stop walk sound immediately when jumping
                PlayJumpSound(); // Play jump sound
            }
        }
    }

    // Function to play the walking sound
    private void PlayWalkSound()
    {
        if (walkSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true; // Loop the walking sound while player moves
            audioSource.volume = targetVolume; // Set volume to target immediately
            audioSource.Play();
        }
    }

    // Function to stop the walking sound
    private void StopWalkSound()
    {
        if (audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop(); // Stop the walking sound immediately
        }
    }

    // Function to play the jump sound (one-time only)
    private void PlayJumpSound()
    {
        if (jumpSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = jumpSound;
            audioSource.loop = false; // Ensure jump sound doesn't loop
            audioSource.volume = targetVolume; // Set volume to target immediately
            audioSource.Play();
        }
    }

    // Check if grounded based on vertical velocity
    private void FixedUpdate()
    {
        if (rb.velocity.y == 0) // If vertical velocity is zero, player is grounded
        {
            if (!isGrounded)
            {
                isGrounded = true; // Player is grounded
                hasJumped = false; // Reset jump flag after landing

                // Resume walk sound if the player is grounded and moving
                if (isWalking && !audioSource.isPlaying) // If the player is walking and sound is not playing, resume walk sound
                {
                    PlayWalkSound();
                }
            }
        }
        else
        {
            isGrounded = false; // Player is not grounded when moving vertically
        }
    }
}
