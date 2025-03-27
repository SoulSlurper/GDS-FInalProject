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
        // Find the player object in the scene (assuming it has the "Player" tag)
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No object with the 'Player' tag found in the scene.");
        }

        // Get the AudioSource component on the same GameObject
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume; // Start with walk sound volume

        // Get the Rigidbody2D component for velocity checking
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player != null)
        {
            // Check if the player is moving using WASD (Horizontal and Vertical input)
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            bool isPlayerMoving = Mathf.Abs(horizontalInput) > speedThreshold || Mathf.Abs(verticalInput) > speedThreshold;

            // Handle walking sound based on player's movement
            if (isPlayerMoving && isGrounded) // Only play walk sound if grounded and moving
            {
                if (!isWalking)
                {
                    isWalking = true;
                    PlayWalkSound();
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

    // Function to play the jump sound (ensure it only plays once per jump)
    private void PlayJumpSound()
    {
        if (jumpSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = jumpSound;
            audioSource.loop = false; // Ensure jump sound does not loop
            audioSource.Play();
        }
    }

    // Check for when the player is grounded and moving with WASD again
    private void FixedUpdate()
    {
        if (player != null)
        {
            // Assuming the player has a Rigidbody2D component and falls under gravity
            if (rb.velocity.y == 0) // Grounded check (no vertical velocity)
            {
                isGrounded = true; // Player is grounded when vertical velocity is zero

                // Reset the jump flag once grounded, so the jump sound can play again next time
                if (hasJumped)
                {
                    hasJumped = false; // Reset jump flag when player lands
                }

                // Resume walk sound if the player is grounded and moving
                if (isWalking && !audioSource.isPlaying && audioSource.clip != walkSound) // If the player is walking and sound is not playing, resume walk sound
                {
                    PlayWalkSound();
                }
            }
            else
            {
                isGrounded = false; // Player is not grounded when moving vertically
            }
        }
    }
}
