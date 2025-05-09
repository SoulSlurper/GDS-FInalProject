using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRespawn : MonoBehaviour
{
    private Status status;
    private SlimeKnightController playerController;
    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Vector2 initialVelocity;
    private Vector3 initialSize;

    private SavePoint savePoint;
    private Camera mainCamera;

    private bool isSceneFrozen = false;
    private float originalTimeScale;

    #region Unity functions
    void Start()
    {
        status = GetComponent<Status>();
        playerController = GetComponent<SlimeKnightController>();
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        initialVelocity = rb.velocity;
        initialSize = transform.localScale;

        mainCamera = Camera.main;
        
        originalTimeScale = Time.timeScale;
    }

    void Update()
    {
        // Handle OnDeath/respawn
        if (status.noHealth || Input.GetKeyDown(KeyCode.R))
        {
            OnDeath();
            OnRevive();
            OnReturn();
        }
    }

    // Handle various collision types
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") || collision.CompareTag("Item") || collision.CompareTag("Enemy")) return;

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
    #endregion

    #region On Scene
    // pauses the scene
    private void PauseScene()
    {
        Time.timeScale = 0f;
        isSceneFrozen = true;
    }

    // resumes the scene
    private void ResumeScene()
    {
        Time.timeScale = originalTimeScale;
        isSceneFrozen = false;
    }

    // Remove boss if present
    private bool RemoveBoss()
    {
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

            return true;
        }

        return false;
    }
    #endregion

    #region Respawn functions
    // when the player dies
    private void OnDeath()
    {
        //PauseScene();

        rb.velocity = initialVelocity;
    }

    // when the player returns to the save point
    private void OnReturn()
    {
        // Move player to save point or initial position
        transform.position = (savePoint != null) ? savePoint.position : initialPosition;

        RemoveBoss();

        //ResumeScene();
    }

    // Player respawn logic
    private void OnRevive()
    {
        if (status.noHealth)
        {
            // Reset health and player state
            status.ResetHealth();
            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
        }
    }
    #endregion
}
