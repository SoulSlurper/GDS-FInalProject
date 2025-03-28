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
    public AudioClip splatterSound;  // New splatter sound

    private AudioSource audioSource;
    private GameObject player;
    private float targetVolume = 0.5f;
    private bool isWalking = false;
    private bool isSplattering = false;  // Flag to track splatter sound

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
            audioSource.loop = true;
            audioSource.volume = targetVolume;
            audioSource.Play();
            StartCoroutine(WaitForSplatterToEnd());
        }
    }

    public bool IsSplattering()
    {
        return isSplattering;
    }

    private IEnumerator WaitForSplatterToEnd()
    {
        // Wait until the splatter sound finishes its loop, then resume the walk sound
        yield return new WaitForSeconds(splatterSound.length);
        isSplattering = false;
        PlayWalkSound();  // Resume walk sound after splatter
    }
}
