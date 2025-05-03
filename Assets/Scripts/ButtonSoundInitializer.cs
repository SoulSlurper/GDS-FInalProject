using UnityEngine;

public class ButtonSoundInitializer : MonoBehaviour
{
    void Start()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<UIButtonSound>() == null)
            {
                button.AddComponent<UIButtonSound>();
            }
        }
    }
}