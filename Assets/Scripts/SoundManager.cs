using System;
using System.Collections;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip backgroundMusic;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip buttonSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip splatterSound;
    public AudioClip swordSound;
    public AudioClip shootSound;
    public AudioClip enemyDetectedSound;
    public AudioClip teleportEnterSound;
    public AudioClip teleportExitSound;
    public AudioClip enemyDashSound;
    public AudioClip wallBreakSound;
    public AudioClip enemyLaserShootSound;
    public AudioClip slimeHitSound;




    private AudioSource musicSource;
    private AudioSource audioSource;
    private GameObject player;
    private float targetVolume = 1f;
    private bool isWalking = false;
    private bool isSplattering = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple SoundManager instances found! Destroying the duplicate.");
            Destroy(gameObject); // Prevent multiple SoundManagers
            return;
        }
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = 0.6f;
        musicSource.playOnAwake = false;
        musicSource.ignoreListenerVolume = true;

        if (backgroundMusic != null)
        {
            musicSource.Play();
        }
    }
    public void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound, targetVolume);
        }
    }

    public void PlayDoorOpenSound()
    {
        if (doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound, targetVolume);
        }
    }

    public void PlayDoorCloseSound()
    {
        if (doorCloseSound != null)
        {
            audioSource.PlayOneShot(doorCloseSound, targetVolume);
        }
    }

    public void PlayWalkSound()
    {
        if (walkSound != null && !isWalking && !isSplattering)
        {
            isWalking = true;
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.volume = targetVolume;
            audioSource.Play();
        }
    }

    public void StopWalkSound()
    {
        if (isWalking)
        {
            isWalking = false;
            audioSource.Stop();
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.clip = jumpSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    
    public void PlaySplatterSound()
    {
        if (splatterSound != null)
        {
            GameObject tempGO = new GameObject("TempSplatterSound");
            tempGO.transform.position = player != null ? player.transform.position : Vector3.zero;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = splatterSound;
            tempSource.volume = targetVolume / 2f;
            tempSource.Play();

            Destroy(tempGO, splatterSound.length);
        }
    }

    public void PlaySwordSound()
    {
        if (swordSound != null)
        {
            GameObject tempGO = new GameObject("TempSwordSound");
            tempGO.transform.position = player != null ? player.transform.position : Vector3.zero;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = swordSound;
            tempSource.volume = targetVolume * 2f;
            tempSource.Play();

            Destroy(tempGO, swordSound.length);
        }
    }

    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            GameObject tempGO = new GameObject("TempShootSound");
            tempGO.transform.position = player != null ? player.transform.position : Vector3.zero;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = shootSound;
            tempSource.volume = targetVolume * 0.6f;
            tempSource.Play();

            Destroy(tempGO, shootSound.length);
        }
    }
    
    public void PlayEnemyDetectedSound()
    {
        if (enemyDetectedSound != null)
        {
            audioSource.PlayOneShot(enemyDetectedSound, targetVolume * 1.2f);
        }
    }
    
    public void PlayTeleportEnterSound()
    {
        if (teleportEnterSound != null)
        {
            audioSource.PlayOneShot(teleportEnterSound, targetVolume);
        }
    }

    public void PlayTeleportExitSound()
    {
        if (teleportExitSound != null)
        {
            audioSource.PlayOneShot(teleportExitSound, targetVolume);
        }
    }
    
    public void PlayEnemyDashSound()
    {
        if (enemyDashSound != null)
        {
            audioSource.PlayOneShot(enemyDashSound, targetVolume * 1.1f);
        }
    }

    public void PlayWallBreakSound(Vector3 position)
    {
        if (wallBreakSound != null)
        {
            GameObject tempGO = new GameObject("TempWallBreakSound");
            tempGO.transform.position = position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = wallBreakSound;
            tempSource.spatialBlend = 0f; // Set to 1 for 3D spatial audio
            tempSource.volume = targetVolume;
            tempSource.Play();

            Destroy(tempGO, wallBreakSound.length);
        }
    }

    public void PlayLaserShootSound(Vector3 position)
    {
        if (enemyLaserShootSound != null)
        {
            GameObject tempGO = new GameObject("TempLaserShootSound");
            tempGO.transform.position = position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = enemyLaserShootSound;
            tempSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
            tempSource.volume = targetVolume;
            tempSource.Play();

            Destroy(tempGO, enemyLaserShootSound.length);
        }
    }

    public void PlaySlimeHitSound(Vector3 position)
    {
        if (slimeHitSound != null)
        {
            GameObject tempGO = new GameObject("TempSlimeHitSound");
            tempGO.transform.position = position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = slimeHitSound;
            tempSource.volume = targetVolume;
            tempSource.Play();

            Destroy(tempGO, slimeHitSound.length);
        }
    }


    // public bool IsSplattering()
    // {
    //     return isSplattering;
    // }

    // private IEnumerator WaitForSplatterToEnd()
    // {
    //     yield return new WaitForSeconds(splatterSound.length);
    //     isSplattering = false;
    //
    //     if (GameObject.FindWithTag("Player") != null)
    //     {
    //         SlimeKnightController playerController = GameObject.FindWithTag("Player").GetComponent<SlimeKnightController>();
    //         if (playerController != null && playerController.IsMoving())
    //         {
    //             PlayWalkSound();  // Resume walking only if the player is still moving
    //         }
    //     }
    // }

}