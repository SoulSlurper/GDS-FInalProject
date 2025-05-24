using System.Collections;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] public GameObject boss;
    [SerializeField] private HealthBar healthBar;

    [Header("Camera Details")]
    [SerializeField] public Camera mainCamera;
    [SerializeField] private float bossCameraSize = 5f;

    public static bool hasSpawnedBoss = false;
    public static bool bossDefeated = false;

    private Collider2D col;
    private float originalCameraSize;

    private GameObject spawnedBoss;

    void Start()
    {
        healthBar.SetActiveState(false);

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        col = GetComponent<Collider2D>();
        originalCameraSize = mainCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasSpawnedBoss && collision.CompareTag("Player"))
        {
            SpawnBoss();
            mainCamera.orthographicSize = bossCameraSize;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBossMusicUntilDefeated(this, () => spawnedBoss == null);
            }

            col.enabled = false;
        }
    }

    private void SpawnBoss()
    {
        spawnedBoss = Instantiate(boss, new Vector2(180f, -2f), Quaternion.identity);
        hasSpawnedBoss = true;

        // Setup health bar if boss has Status
        Status bossStatus = spawnedBoss.GetComponent<Status>();
        if (bossStatus != null)
        {
            bossStatus.SetHealthBar(healthBar);
        }

        healthBar.SetActiveState(true);

        // Start watching for boss defeat
        StartCoroutine(WatchForBossDefeat());
    }

    private IEnumerator WatchForBossDefeat()
    {
        while (spawnedBoss != null)
        {
            yield return null;
        }

        RestoreOriginalCamera();
        healthBar.SetActiveState(false);

        if (!bossDefeated) col.enabled = true;
    }

    public void RestoreOriginalCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = originalCameraSize;
        }

        hasSpawnedBoss = false;
    }
}
