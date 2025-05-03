using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private ElectricBarrier linkedBarrier;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        if (collision.CompareTag("Player"))
        {
            ActivateSwitch();
        }
    }

    private void ActivateSwitch()
    {
        isActivated = true;
        if (linkedBarrier) linkedBarrier.DeactivateBarrier();
        Debug.Log("Switch activated!");
        // Optional: Play animation, sound, change switch appearance, etc.
    }
}
