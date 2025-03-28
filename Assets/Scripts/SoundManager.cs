using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip buttonSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    private AudioSource audioSource; 
    private GameObject player;
    private float targetVolume = 0.5f;
    private bool isWalking = false;

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
        if (walkSound != null && !isWalking)
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
}
