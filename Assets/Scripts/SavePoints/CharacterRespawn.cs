using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRespawn : MonoBehaviour
{
    private Status status;
    private SlimeKnightController playerController;
    private Vector2 initialPosition;

    private SavePoint savePoint;
    private Camera mainCamera;

    void Start()
    {
        status = GetComponent<Status>();
        playerController = GetComponent<SlimeKnightController>();
        initialPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Handle death/respawn
        if (status.noHealth || Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    // Handle various collision types
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Save point logic
        if (collision.CompareTag("SavePoint"))
        {
            SavePoint encounteredSavePoint = collision.gameObject.GetComponent<SavePoint>();
            if (encounteredSavePoint != null && !encounteredSavePoint.Equals(savePoint))
            {
                if (savePoint) savePoint.isActive = false;
                savePoint = encounteredSavePoint;
                savePoint.isActive = true;
                Debug.Log("SavePoint Recorded: " + savePoint.position);
            }
        }
    }

    // Player respawn logic
    private void Respawn()
    {
        // Move player to save point or initial position
        transform.position = (savePoint != null) ? savePoint.position : initialPosition;

        // If player died, reset everything
        if (status.noHealth)
        {
            // Remove boss if present
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss)
            {
                boss.GetComponent<Status>().healthBar.SetActiveState(false);
                Destroy(boss);

                // Reset camera
                mainCamera.orthographicSize = 3f;
                BossTrigger.hasSpawnedBoss = false;

                // Reset any background music
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.ResumeBackgroundMusic();
                }
            }

            // Reset health and player state
            status.ResetHealth();
            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
        }
    }

}
