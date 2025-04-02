using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeWeapon : Weapon
{
    [Header("Close Range Details")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;


    // Getter and Setters // // // //


    // Unity // // // // //
    void Start()
    {
        col.enabled = false;
    }

    void Update()
    {
        PerformAttack();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger detects: " + collision.gameObject.name);

        MakeDamage(collision);

        col.enabled = false;
    }



    // Attack Details // // // // //
    public override void Attack()
    {
        col.enabled = true;

        animator.SetTrigger("Attack");

        if (SoundManager.Instance != null)
        {
            Debug.Log("Attempting to play sword sound");
            SoundManager.Instance.PlaySwordSound();
        }
        else
        {
            Debug.LogWarning("SoungManager instance is null");
        }
    }
}
