using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterRespawn : MonoBehaviour
{
    [SerializeField] private float shrinkDuration = 1f; //the time taken when the player shrinks
    [SerializeField] private float cameraTravelDuration = 1f; //the time taken when the player shrinks

    //player details
    private Status status;
    private SlimeKnightController playerController;
    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Vector2 initialVelocity;
    private Vector3 initialSize;

    //on scene details
    private SavePoint savePoint;
    private Camera mainCamera;

    //time details
    private bool isSceneFrozen = false;
    private float originalTimeScale;

    //lerping details
    private float t = 0f;
    private Vector3 startLerpScale;
    private Vector3 endLerpScale;

    private bool isDespawning = false;

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
        // Handle death/respawn
        if (!isSceneFrozen && (status.noHealth || Input.GetKeyDown(KeyCode.R)))
        {
            PauseScene();

            Revive();

            OnDespawning();
        }

        // the process of respawning in frozen time
        if (isSceneFrozen)
        {
            t += Time.unscaledDeltaTime / shrinkDuration;
            transform.localScale = Vector3.Lerp(startLerpScale, endLerpScale, t);

            if (t > 1f)
            {
                if (isDespawning) OnSpawning(); //switches to when the player spawns at the savepoint or beginning position
                else ResumeScene();
            }
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

    //positions the camera as the camera from the CameraFollow script does not follow in FixedUpdate
    private IEnumerator PositionCameraToCharacter(float duration)
    {
        Vector3 startScale = mainCamera.transform.position;
        Vector3 endScale = transform.position;
        endScale.z = startScale.z; //maintains the camera view

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            mainCamera.transform.position = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = endScale;
    }
    #endregion

    #region Respawn functions
    // restores the player's health and scene if the player does not have any health left
    private void Revive()
    {
        if (status.noHealth)
        {
            RemoveBoss();

            // Reset health and player state
            status.ResetHealth();
            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
        }
    }

    // when the player dies, where it sets up the player and lerping details
    private void OnDespawning()
    {
        initialSize = transform.localScale;
        rb.velocity = initialVelocity;

        startLerpScale = initialSize;
        endLerpScale = Vector3.zero;
        t = 0f;

        isDespawning = true;
    }

    // when the player returns to the save point, where it sets up the player and lerping details
    private void OnSpawning()
    {
        // Move player to save point or initial position
        transform.position = (savePoint != null) ? savePoint.position : initialPosition;

        StartCoroutine(PositionCameraToCharacter(cameraTravelDuration));

        startLerpScale = transform.localScale;
        endLerpScale = initialSize;
        t = 0f;

        isDespawning = false;
    }
    #endregion
}
