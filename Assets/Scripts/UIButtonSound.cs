using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    [Header("Optional Custom Sound")]
    public AudioClip customButtonSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (customButtonSound != null)
        {
            SoundManager.Instance?.PlayCustomButtonSound(customButtonSound);
        }
        else if (CompareTag("Button"))
        {
            SoundManager.Instance?.PlayButtonSound();
        }
    }
}