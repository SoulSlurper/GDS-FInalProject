using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnakeBossManager : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Attack1,
        Attack2,
        Attack3,
        Dead
    }

    public BossState currentState;
    public GameObject Head;
    public GameObject Tail;

    public GameObject Player;
    public Vector2 playerPos;

    private float idleTime = 3f;
    private float timer = 0f;
    private bool canAttack = false;
    private bool tailInPos = false;
    private float tailStartY;
    private float tailAttackTime = 4f;
    public bool tailCanAttack = false;
    

    void Start()
    {
        currentState = BossState.Idle;
        Debug.Log("Boss Idle");
        Player = GameObject.FindGameObjectWithTag("Player");
        tailStartY = Tail.GetComponent<Rigidbody2D>().position.x - 10;

    }

    void Update()
    {
        switch (currentState) 
        { 
            case BossState.Idle:
                idleState();
                break;
            case BossState.Attack1:
                attack1();
                break;
            case BossState.Attack2:
                attack2();
                break;
            case BossState.Attack3:
                attack3();
                break;
            case BossState.Dead:
                dead();
                break;
        }

        bossUpdates();
    }

    void bossUpdates()
    {
        /*Check if dead
            NEED TO DO
        */

        //Update the players position
        playerPos = Player.GetComponent<Rigidbody2D>().position;

        if (!canAttack) { return; }

        //reset timer and canAttack
        timer = 0f;
        tailAttackTime = 0f;
        canAttack = false;

        switch(/*Random.Range(0,4)*/1)
        {
            case 1:
                currentState = BossState.Attack1;
                Debug.Log("Attack 1");
                break;
            case 2:
                currentState = BossState.Attack2;
                Debug.Log("Attack 2");
                break;
            case 3:
                currentState = BossState.Attack3;
                Debug.Log("Attack 3");
                break;
        }
    }

    void idleState()
    {
        timer += Time.deltaTime;

        if (timer > idleTime)
        {
            canAttack = true;
            Debug.Log("Timer Finished");
        }
    }

    void dead()
    {

    }

    void attack1()
    {
        Rigidbody2D tailRB = Tail.GetComponent<Rigidbody2D>();
        BoxCollider2D tailCollider = Tail.gameObject.GetComponent<BoxCollider2D>();
        tailCollider.isTrigger = true;
        Vector2 tailPos;
        
        tailPos = new Vector2(playerPos.x, tailStartY);
        tailRB.MovePosition(tailPos);

        if (tailCanAttack)
        {
            tailRB.velocityY = 4f;
            currentState = BossState.Idle;
            Debug.Log("Boss Idle");
            tailCanAttack = false;
        }
    }

    void attack2()
    {
        currentState = BossState.Idle;
        Debug.Log("Boss Idle");
    }

    void attack3()
    {
        currentState = BossState.Idle;
        Debug.Log("Boss Idle");
    }

}
