using UnityEngine;

public class AnimationButton : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string stateParameter = "On";
    [SerializeField] private bool toggleMode = true;

    [Header("Collider Settings")]
    [SerializeField] private Collider2D doorCollider; // Assign the door's collider here
    private bool originalColliderEnabled; // To remember initial state

    private bool isOn = false;
    private int objectsOnButton = 0;

    private void Start()
    {
        // Store the initial collider state
        if (doorCollider != null)
        {
            originalColliderEnabled = doorCollider.enabled;
        }
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

            // Handle collider state
            UpdateColliderState();
        }
    }

    private void UpdateColliderState()
    {
        if (doorCollider != null)
        {
            // Disable collider when door is open, restore to original state when closed
            doorCollider.enabled = !isOn && originalColliderEnabled;
        }
    }

    // For external control if needed
    public void ForceSetState(bool openState)
    {
        SetState(openState);
    }
}