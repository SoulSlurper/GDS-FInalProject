using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnakeBossManager : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        TailAttack,
        BiteAttack,
        ShootAttack,
        Dead
    }

    public BossState currentState;
    public GameObject Head;
    public GameObject Tail;

    public GameObject Player;
    public Vector2 playerPos;

    private float idleTime = 3f;
    float idleTimer = 0f;

    public SnakeTailManager TailManager;
    public SnakeHeadManager HeadManager;

    void Start()
    {
        currentState = BossState.Idle;
        Debug.Log("Boss Idle");
        Player = GameObject.FindGameObjectWithTag("Player");
        
        HeadManager = Head.GetComponent<SnakeHeadManager>();
        TailManager = Tail.GetComponent<SnakeTailManager>();
    }

    void FixedUpdate()
    {
        playerPos = Player.GetComponent<Rigidbody2D>().position;


        switch (currentState) 
        { 
            case BossState.Idle:
                idleState();
                break;
            case BossState.TailAttack:
                tailAttack();
                TailManager.tailAttack(playerPos);
                HeadManager.tailAttack(playerPos);
                break;
            case BossState.BiteAttack:
                biteAttack();
                HeadManager.biteAttack();
                TailManager.biteAttack();
                break;
            case BossState.ShootAttack:
                shootAttack();
                HeadManager.shootAttack();
                TailManager.shootAttack();
                break;
            case BossState.Dead:
                dead();
                break;
        }

    }

    void pickState()
    {
        /*switch (Random.Range(0, 3))
        {
            case 0:
                currentState = BossState.TailAttack; break;
            case 1:
                currentState = BossState.BiteAttack; break;
            case 2:
                currentState = BossState.ShootAttack; break;
        }*/
        currentState = BossState.TailAttack;
        Debug.Log(currentState);
    }

    void idleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > idleTime) 
        {
            pickState();
            idleTimer = 0f;
        }
    }

    void dead()
    {

    }

    void tailAttack()
    {
        if (TailManager.tailAttackDone) 
        {
            currentState = BossState.Idle;
            Debug.Log(currentState);
            TailManager.tailAttackDone = false;
        }
    }

    void biteAttack()
    {
        
    }

    void shootAttack()
    {
       
    }

}
