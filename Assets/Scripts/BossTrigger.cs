using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    public GameObject boss;
    [SerializeField]
    public Camera mainCamera;
    [SerializeField]
    private float bossCameraSize = 5f;
    
    public static bool hasSpawnedBoss = false;
    private float originalCameraSize;
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        originalCameraSize = mainCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasSpawnedBoss && collision.CompareTag("Player"))
        {
            Instantiate(boss, new Vector2(45f, 2f), Quaternion.identity);
            hasSpawnedBoss = true;
            
            // Set camera size for boss battle
            mainCamera.orthographicSize = bossCameraSize;
            
            // Optional: Disable the trigger after spawning
            //gameObject.SetActive(false);
        }
    }
    
    // Method to restore original camera settings (can be called when boss is defeated)
    public void RestoreOriginalCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = originalCameraSize;
        }
        
        // Reset the static flag if needed for future boss encounters
        hasSpawnedBoss = false;
    }
}