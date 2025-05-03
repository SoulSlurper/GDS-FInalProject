using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Fade Settings")]
    public Image fadeImage; // Drag the full-screen black Image here
    public float fadeDuration = 1f;

    public void LoadSceneWithFade(string sceneName)
    {
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

        // Ensure alpha starts at 0
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
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