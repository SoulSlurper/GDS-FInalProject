using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeWeapon : Weapon
{
    [Header("Close Range Details")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float _raycastRadius = 1f;


    // Getter and Setters // // // //
    public float raycastRadius
    {
        get { return _raycastRadius; }
        private set { _raycastRadius = value; }
    }


    // Unity // // // // //
    void Update()
    {
        PerformAttack();
    }

    void OnDrawGizmosSelected()
    {
        //shows the raycast range when the gizmos is selected in the scene

        Gizmos.color = Color.blue;
        Vector3 position = raycastOrigin == null ? Vector3.zero : raycastOrigin.position;
        Gizmos.DrawWireSphere(position, raycastRadius);
    }



    // Close Range Details // // // // //
    public void SetRaycastRadius(float radius) { raycastRadius =  radius; }



    // Attack Details // // // // //
    public override void Attack(Collider2D collider = null)
    {
        MakeDamage();

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

    private void MakeDamage()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(raycastOrigin.position, raycastRadius))
        {
            Debug.Log(collider.name + " - " + collider.tag);

            if (!collider.CompareTag("Player"))
            {
                Status status;
                if (status = collider.GetComponent<BossHp>())
                {
                    status.health.DecreaseAmount(damage);
                }
                else if (status = collider.GetComponent<EnemyStatus>())
                {
                    status.health.DecreaseAmount(damage);
                }
                else if (status = collider.GetComponent<BreakableWall>())
                {
                    status.health.DecreaseAmount(damage);
                }
            }
        }
    }
}
