using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    public Animator animator;
    public float minTimeBetweenStrikes = 0.5f;
    public float maxTimeBetweenStrikes = 2f;

    public float minScale = 1f;
    public float maxScale = 1.5f;

    private float timer;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            TriggerRandomLightning();
            ResetTimer();
        }
    }

    private void TriggerRandomLightning()
    {
        int randomAttack = Random.Range(1, 7); // 1 to 6 inclusive
        animator.SetTrigger($"Lightning{randomAttack}");

        // Randomly scale lightning
        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // More frequent flicker: 50% chance
        if (Random.value < 0.5f)
        {
            Invoke(nameof(TriggerRandomLightning), Random.Range(0.05f, 0.15f));

            // Chance for a third strike after the second
            if (Random.value < 0.3f) // 30% chance for a third
            {
                Invoke(nameof(TriggerRandomLightning), Random.Range(0.15f, 0.25f));
            }
        }
    }

    private void ResetTimer()
    {
        timer = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
    }
}


