using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip walkSound; // Walking sound clip
    public AudioClip jumpSound; // Jump sound clip
    private AudioSource audioSource; // Audio source for playing sounds
    private GameObject player; // Reference to the player GameObject
    private bool isWalking = false; // To check if the player is walking
    private float targetVolume = 0.5f; // Reduced volume (half of 1)
    private float maxPlayerSpeed = 5f; // Max speed of the player for volume scaling
    private float speedThreshold = 0.1f; // Speed threshold for movement detection
    private bool isGrounded = true; // To check if the player is grounded

    private void Start()
    {
        // Find the player object in the scene (assuming it has the "Player" tag)
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No object with the 'Player' tag found in the scene.");
        }

        // Get the AudioSource component on the same GameObject
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0; // Start with volume off
    }

    private void Update()
    {
        if (player != null)
        {
            // Check if the player is moving using their velocity (for smoother detection)
            float playerSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude; // Get the speed of the player

            // Check if player is moving and update sound accordingly
            if (playerSpeed > speedThreshold) // Consider the player is moving if their speed is above the threshold
            {
                if (!isWalking)
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
                // Stop the walking sound immediately when the player stops moving
                if (isWalking)
                {
                    isWalking = false;
                    StopWalkSound();
                }
            }

            // Check if the player is jumping and is grounded
            if (Input.GetButtonDown("Jump") && isGrounded) // You can replace this with your jump condition
            {
                isGrounded = false; // Player is no longer grounded after jumping
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
        audioSource.Stop(); // Stop the walking sound immediately
    }

    // Function to play the jump sound
    private void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.clip = jumpSound;
            audioSource.volume = targetVolume; // Set volume to target immediately
            audioSource.Play();
        }
    }

    // You should update the ground check based on your own system.
    // For now, this is a simple check based on the player's vertical velocity.
    private void FixedUpdate()
    {
        if (player != null)
        {
            // Assuming the player has a Rigidbody2D component and falls under gravity
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            // Check if the player is grounded by looking at the velocity
            if (rb.velocity.y == 0)
            {
                isGrounded = true; // Player is grounded when vertical velocity is zero
            }
            else
            {
                isGrounded = false; // Player is not grounded when moving vertically
            }
        }
    }
}
