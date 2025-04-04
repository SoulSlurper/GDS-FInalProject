using UnityEngine;

public class AnimationButton : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string stateParameter = "On";
    [SerializeField] private bool toggleMode = true;

    [Header("Collider Settings")]
    [SerializeField] private Collider2D doorCollider;
    private bool originalColliderEnabled;

    private bool isOn = false;
    private int objectsOnButton = 0;
    private SoundManager soundManager;

    private void Start()
    {
        if (doorCollider != null)
        {
            originalColliderEnabled = doorCollider.enabled;
        }

        soundManager = FindObjectOfType<SoundManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidTrigger(other))
        {
            objectsOnButton++;

            if (toggleMode)
            {
                if (objectsOnButton == 1) ToggleState();
            }
            else
            {
                SetState(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidTrigger(other))
        {
            objectsOnButton--;
            if (!toggleMode && objectsOnButton <= 0)
            {
                SetState(false);
            }
        }
    }

    private bool IsValidTrigger(Collider2D collider)
    {
        return collider.CompareTag("Player") || collider.CompareTag("MovableObject");
    }

    private void ToggleState()
    {
        SetState(!isOn);
    }

    private void SetState(bool newState)
    {
        if (isOn != newState && targetAnimator != null)
        {
            isOn = newState;
            targetAnimator.SetBool(stateParameter, isOn);
            UpdateColliderState();
            if (soundManager != null)
            {
                soundManager.PlayButtonSound();
                if (isOn)
                    soundManager.PlayDoorOpenSound();
                else
                    soundManager.PlayDoorCloseSound();
            }
        }
    }

    private void UpdateColliderState()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = !isOn && originalColliderEnabled;
        }
    }

    public void ForceSetState(bool openState)
    {
        SetState(openState);
    }
}
