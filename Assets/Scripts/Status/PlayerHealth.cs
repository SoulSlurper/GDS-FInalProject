using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Status
{
    //[SerializeField] private StatusAmount _stamina;

    [SerializeField]
    public int hp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossProjectile"))
        {
            hp -= 1;
            respawn();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Boss"))
        {
            hp -= 1;
            respawn();
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            hp -= 1;
            respawn();
        }
    }

    public override StatusType GetStatusType() { return StatusType.Player; }

    public void respawn()
    {
        GameObject Boss = GameObject.FindWithTag("Boss");
        if (hp <= 0 && Boss)
        {

            this.transform.position = new Vector2(117, -10);
            Destroy(Boss);
            hp = 3;
            BossTrigger.hasSpawnedBoss = false;

        }
        else if (hp <= 0 && !Boss)
        {
            this.transform.position = new Vector2(22, -1);
            hp = 3;
        }

    }
}
