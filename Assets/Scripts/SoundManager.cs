using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip bossMusic;
    
    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip UIButtonSound;
    
    [Header("Environment Sounds")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    [SerializeField] private AudioClip wallBreakSound;
    [SerializeField] private AudioClip teleportEnterSound;
    [SerializeField] private AudioClip teleportExitSound;
    
    [Header("Player Sounds")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip splatterSound;
    [SerializeField] private AudioClip swordSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip slimeHitSound;
    
    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyDetectedSound;
    [SerializeField] private AudioClip enemyDashSound;
    [SerializeField] private AudioClip enemyLaserShootSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.6f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1.0f;

    [Header("Audio Source Pool")]
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private int maxPoolSize = 15;

    // Audio sources
    private AudioSource musicSource;
    private AudioSource loopSource;
    
    // Pooling system
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();
    
    // State tracking
    private bool isWalking = false;
    private bool isSplattering = false;
    private GameObject player;
    
    
    private Coroutine bossMusicRoutine;


    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundSystem();
        }
        else
        {
            Debug.LogWarning("Multiple SoundManager instances found! Destroying the duplicate.");
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Get player reference
        player = GameObject.FindWithTag("Player");
        
        // Play background music
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic, musicVolume);
        }
    }
    
    private void Update()
    {
        // Cleanup finished audio sources
        CleanupFinishedAudioSources();
    }
    
    private void InitializeSoundSystem()
    {
        // Create music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.ignoreListenerVolume = true;
        
        // Create loop source for walking etc.
        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;
        
        // Initialize audio source pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePooledAudioSource();
        }
    }
    
    private AudioSource CreatePooledAudioSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.loop = false;
        audioSourcePool.Enqueue(newSource);
        return newSource;
    }
    
    private AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count == 0 && activeAudioSources.Count < maxPoolSize)
        {
            return CreatePooledAudioSource();
        }
        
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            activeAudioSources.Add(source);
            return source;
        }
        
        // If pool is empty and at max capacity, reuse the oldest active source
        AudioSource oldestSource = activeAudioSources[0];
        activeAudioSources.RemoveAt(0);
        activeAudioSources.Add(oldestSource);
        oldestSource.Stop();
        return oldestSource;
    }
    
    private void CleanupFinishedAudioSources()
    {
        for (int i = activeAudioSources.Count - 1; i >= 0; i--)
        {
            if (!activeAudioSources[i].isPlaying)
            {
                audioSourcePool.Enqueue(activeAudioSources[i]);
                activeAudioSources.RemoveAt(i);
            }
        }
    }
    
    #region Public Sound Methods
    
    public void PlayBossMusicUntilDefeated(MonoBehaviour caller, System.Func<bool> isBossDefeated)
    {
        if (bossMusicRoutine != null)
        {
            caller.StopCoroutine(bossMusicRoutine);
        }

        bossMusicRoutine = caller.StartCoroutine(WaitForBossDefeat(isBossDefeated));
    }

    
    // UI Sounds
    public void PlayButtonSound()
    {
        PlaySound(buttonSound, sfxVolume);
    }
    public void PlayCustomButtonSound(AudioClip clip)
    {
        if (clip != null)
        {
            PlaySound(clip, sfxVolume);
        }
    }

    
    // Environment Sounds
    public void PlayDoorOpenSound()
    {
        PlaySound(doorOpenSound, sfxVolume);
    }
    
    public void PlayDoorCloseSound()
    {
        PlaySound(doorCloseSound, sfxVolume);
    }
    
    public void PlayWallBreakSound(Vector3 position)
    {
        PlaySoundAtPosition(wallBreakSound, position, sfxVolume);
    }
    
    public void PlayTeleportEnterSound()
    {
        PlaySound(teleportEnterSound, sfxVolume);
    }
    
    public void PlayTeleportExitSound()
    {
        PlaySound(teleportExitSound, sfxVolume);
    }
    
    // Player Sounds
    public void PlayWalkSound()
    {
        if (walkSound != null && !isWalking && !isSplattering)
        {
            isWalking = true;
            loopSource.clip = walkSound;
            loopSource.volume = sfxVolume * masterVolume;
            loopSource.Play();
        }
    }
    
    public void StopWalkSound()
    {
        if (isWalking)
        {
            isWalking = false;
            loopSource.Stop();
        }
    }
    
    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            loopSource.Stop();
            PlaySound(jumpSound, sfxVolume);
        }
    }
    
    public void PlaySplatterSound()
    {
        if (splatterSound != null)
        {
            PlaySoundAtPosition(splatterSound, GetPlayerPosition(), sfxVolume * 0.5f);
        }
    }
    
    public void PlaySwordSound()
    {
        if (swordSound != null)
        {
            PlaySoundAtPosition(swordSound, GetPlayerPosition(), sfxVolume * 2f);
        }
    }
    
    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            PlaySoundAtPosition(shootSound, GetPlayerPosition(), sfxVolume * 5f);
        }
    }
    
    public void PlaySlimeHitSound(Vector3 position)
    {
        if (slimeHitSound != null)
        {
            PlaySoundAtPosition(slimeHitSound, position, sfxVolume);
        }
    }
    
    // Enemy Sounds
    public void PlayEnemyDetectedSound()
    {
        PlaySound(enemyDetectedSound, sfxVolume * 1.2f);
    }
    
    public void PlayEnemyDashSound()
    {
        PlaySound(enemyDashSound, sfxVolume * 1.1f);
    }
    
    public void PlayEnemyLaserShootSound(Vector3 position)
    {
        PlaySoundAtPosition(enemyLaserShootSound, position, sfxVolume);
    }
    
    // Music Control
    public void PlayBossMusic()
    {
        if (musicSource != null && bossMusic != null)
        {
            StartCoroutine(FadeOutAndSwitchMusic(bossMusic, 1.5f, musicVolume * 1.15f));
        }
    }
    
    public void ResumeBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            StartCoroutine(FadeOutAndSwitchMusic(backgroundMusic, 1.5f, musicVolume));
        }
    }
    
    #endregion
    
    #region Private Helper Methods
    
    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null)
            return;
            
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.spatialBlend = 0f; // 2D sound
        source.Play();
    }
    
    private void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null)
            return;
            
        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.spatialBlend = 0.5f; // Mix of 2D and 3D sound
        source.Play();
    }
    
    private Vector3 GetPlayerPosition()
    {
        return player != null ? player.transform.position : Vector3.zero;
    }
    
    private void PlayMusic(AudioClip music, float volume)
    {
        musicSource.clip = music;
        musicSource.volume = volume * masterVolume;
        musicSource.Play();
    }
    
    private IEnumerator FadeOutAndSwitchMusic(AudioClip newMusic, float fadeDuration, float targetVolume)
    {
        float startVolume = musicSource.volume;

        // Fade out current music
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Change clip and reset volume
        musicSource.Stop();
        musicSource.clip = newMusic;
        musicSource.volume = 0f;
        musicSource.Play();

        // Fade in new music
        while (musicSource.volume < targetVolume * masterVolume)
        {
            musicSource.volume += targetVolume * masterVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = targetVolume * masterVolume;
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.volume = sfxVolume * masterVolume;
        }
    }
    
    private void UpdateAllVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.volume = sfxVolume * masterVolume;
        }
        
        // Active sound effects will update on their next play
    }
    
    private IEnumerator WaitForBossDefeat(System.Func<bool> isBossDefeated)
    {
        PlayBossMusic();

        while (!isBossDefeated())
        {
            yield return null;
        }

        ResumeBackgroundMusic();
    }

    
    #endregion
}