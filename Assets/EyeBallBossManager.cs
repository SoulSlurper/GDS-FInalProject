using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EyeBallBossManager : MonoBehaviour
{

    public enum BossState
    {
        Idle,
        RangedAttack,
        Shooting,
        MeleeAttack,
        SummonAttack,
        Dead
    }

    //Boss Variables
    public BossState currentState;
    Rigidbody2D rb;
    public float moveSpeed = 2f;
    [SerializeField]
    public float idleDistance = 5f;

    public float idleTimer = 0f;
    public Vector2 rangedAttackPos;
    public bool rangedPosSet = false;

    [SerializeField]
    public GameObject Laser;

    //Player Variables
    public GameObject Player;
    public Vector2 playerPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = BossState.Idle;
        Debug.Log("Boss Idle");
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case BossState.Idle:
                Idle();
                break;
            case BossState.RangedAttack:
                RangedAttack();
                break;
            case BossState.Shooting:
                break;
            case BossState.MeleeAttack:
                MeleeAttack();
                break;
            case BossState.SummonAttack:
                SummonAttack(); 
                break;
            case BossState.Dead:
                Dead();
                break;
        }
    }

    void pickState()
    {
        /*switch (Random.Range(0, 3))
        {
            case 0:
                currentState = BossState.RangedAttack; break;
            case 1:
                currentState = BossState.MeleeAttack; break;
            case 2:
                currentState = BossState.SummonAttack; break;

        }*/
        currentState = BossState.RangedAttack;
    }
    void Idle()
    {
        if (idleTimer > 7.5f)
        {
            pickState();
            Debug.Log(currentState);
        }
        playerPos = Player.GetComponent<Rigidbody2D>().position;
        Vector2 currentPos = rb.position;
        Vector2 newPos = Vector2.Lerp(currentPos, (playerPos + new Vector2(0, 2)), 1f*Time.deltaTime);
        rb.MovePosition(newPos);

        idleTimer += Time.deltaTime;

    }

    void RangedAttack()
    {
        if (!rangedPosSet)
        {
            rangedAttackPos = this.GetComponent<Rigidbody2D>().position + new Vector2(0, 3);
            rangedPosSet = true;
        }
        
        Vector2 currentPos = rb.position;
        Vector2 newPos = Vector2.Lerp(currentPos, rangedAttackPos, 8f*Time.deltaTime);
        rb.MovePosition(newPos);

        if (currentPos == newPos)
        {
            currentState = BossState.Shooting;
            StartCoroutine(shootLaser());
            Debug.Log(currentState);
        }
    }

    /*void Shooting()
    {
        
    }*/

    IEnumerator shootLaser()
    {
        float attackDuration = 5f;
        float shotTimer = 0f;

        while (shotTimer < attackDuration) 
        {
            Vector2 shootDirection = (Player.GetComponent<Rigidbody2D>().position - rb.position).normalized;
            GameObject laser = Instantiate(Laser, rb.position, Quaternion.identity);

            laser.GetComponent<Rigidbody2D>().velocity = shootDirection * 7.5f;

            yield return new WaitForSeconds(0.5f);

            shotTimer += 0.5f;
        }

        idleTimer = 0f;
        currentState = BossState.Idle;

    }

    void MeleeAttack() 
    { 

    }

    void SummonAttack() 
    {

    }

    void Dead()
    {

    }

}
