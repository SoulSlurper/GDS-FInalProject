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
    public Animator animator;

    private bool isOn = false;
    private int objectsOnButton = 0;
    private SoundManager soundManager;
    private bool hasBeenActivated = false; // New flag to track activation

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
        if (!hasBeenActivated && IsValidTrigger(other)) // Check if not activated yet
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
        if (!hasBeenActivated && IsValidTrigger(other)) // Check if not activated yet
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
        if (!hasBeenActivated && isOn != newState && targetAnimator != null) // Check if not activated yet
        {
            isOn = newState;
            targetAnimator.SetBool(stateParameter, isOn);
            if (animator != null)
            {
                animator.SetBool("ButtonOn", true); // <- This line right here
            }
            UpdateColliderState();
            if (soundManager != null)
            {
                soundManager.PlayButtonSound();
                if (isOn)
                    soundManager.PlayDoorOpenSound();
                else
                    soundManager.PlayDoorCloseSound();
            }

            // Set the flag to true after first activation
            hasBeenActivated = true;
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
        if (!hasBeenActivated) // Check if not activated yet
        {
            SetState(openState);
        }
    }
}
