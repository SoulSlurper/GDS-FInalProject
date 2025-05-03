using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Fade Settings")]
    public Image fadeImage; // Assign your black full-screen image here
    public float fadeDuration = 1.5f;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Awake()
    {
        // Optional: prevent this object from being duplicated across scenes
        // Uncomment if you want this persistent:
        // DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time and hide pause menu when a new scene loads
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        // Hide fade image after scene load (optional)
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        Time.timeScale = 1f; // Reset in case we're paused
        if (fadeImage != null)
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
        else
        {
            Debug.LogWarning("Fade Image not assigned!");
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        Color color = fadeImage.color;
        float elapsed = 0f;

        // Ensure alpha is 0 and fade image is visible
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);

        // Fade to black
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
