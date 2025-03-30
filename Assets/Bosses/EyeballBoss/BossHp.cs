using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHp : Status
{
    [SerializeField]
    public int hp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (noHealth) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Weapon"))
        //{
        //    hp -= 1;
            
        //}
        //if (hp <= 0)
        //{
        //    Destroy(gameObject);
        //}

    }

    public override StatusType GetStatusType() { return StatusType.Boss; }
}
