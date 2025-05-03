using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeWeapon : Weapon
{
    [Header("Close Range Details")]
    [SerializeField] public Animator animator; //sprite and collider animator

    #region Unity
    void Update()
    {
        PerformAttack();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("trigger detects: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Weapon")) return;

        MakeDamage(collision);
    }
    #endregion

    #region Attack Details
    public override void Attack()
    {
        int randomAttack = Random.Range(1, 5);

        switch (randomAttack)
        {
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            case 3:
                animator.SetTrigger("Attack3");
                break;
            case 4:
                animator.SetTrigger("Attack4");
                break;
        }

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
    #endregion
}
