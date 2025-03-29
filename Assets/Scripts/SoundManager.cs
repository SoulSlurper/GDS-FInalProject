using System;
using System.Collections;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip buttonSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip splatterSound;
    public AudioClip swordSound;
    public AudioClip shootSound;

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

    // Method to play the splatter sound and ensure it completes a full loop
    public void PlaySplatterSound()
    {
        if (splatterSound != null && !isSplattering)
        {
            isSplattering = true;
            audioSource.clip = splatterSound;
            audioSource.loop = false;
            audioSource.volume = targetVolume/2;
            audioSource.Play();
            StartCoroutine(WaitForSplatterToEnd());
        }
    }
    public bool IsSplattering()
    {
        return isSplattering;
    }

    public void PlaySwordSound()
    {
        if (swordSound != null)
        {
            audioSource.PlayOneShot(swordSound, targetVolume * 2f);
        }
    }
    public void PlayShootSound()
    {
        if (swordSound != null)
        {
            audioSource.PlayOneShot(shootSound, targetVolume * 6f / 10f);
        }
    }

    private IEnumerator WaitForSplatterToEnd()
    {
        yield return new WaitForSeconds(splatterSound.length);
        isSplattering = false;

        if (GameObject.FindWithTag("Player") != null)
        {
            SlimeKnightController playerController = GameObject.FindWithTag("Player").GetComponent<SlimeKnightController>();
            if (playerController != null && playerController.IsMoving())
            {
                PlayWalkSound();  // Resume walking only if the player is still moving
            }
        }
    }

}