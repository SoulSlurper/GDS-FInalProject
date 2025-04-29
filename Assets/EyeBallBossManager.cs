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
        MeleeAttackStart,
        Charge1,
        Charge2,
        Charge3,
        SummonAttack,
        Dead
    }

    //Boss Variables
    public BossState currentState;
    Rigidbody2D rb;
    [SerializeField]
    private float idleDistance = 5f;

    //Ranged
    private float idleTimer = 0f;
    private Vector2 rangedAttackPos;
    private bool rangedPosSet = false;

    //Melee
    private float chargeTimer = 0f;
    private bool chargePosSet = false;
    Vector2 charge1Start;

    //Summon
    [SerializeField]
    public GameObject eyeball;
    private int eyeballCount = 2;

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
            case BossState.MeleeAttackStart:
                MeleeAttackStart();
                break;
            case BossState.Charge1:
                Charge1();
                break;
            case BossState.Charge2:
                Charge2();
                break;
            case BossState.Charge3:
                Charge3();
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
        /*switch (Random.Range(0, 2))
        {
            case 0:
                currentState = BossState.RangedAttack; break;
            case 1:
                currentState = BossState.MeleeAttackStart; break;
            case 2:
                currentState = BossState.SummonAttack; break;

        }*/
        currentState = BossState.SummonAttack;
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

            laser.GetComponent<Rigidbody2D>().velocity = shootDirection * 8f;

            yield return new WaitForSeconds(0.5f);

            shotTimer += 0.5f;
        }

        idleTimer = 0f;
        rangedPosSet = false;
        currentState = BossState.Idle;

    }

    void MeleeAttackStart() 
    {

        chargeTimer += Time.deltaTime;
        playerPos = Player.GetComponent<Rigidbody2D>().position;
        charge1Start = playerPos + new Vector2(10, -1);
        Vector2 currentPos = rb.position;

        Vector2 newPos = Vector2.Lerp(currentPos, charge1Start, 10f * Time.deltaTime);

        rb.MovePosition(newPos);

        if (chargeTimer > 1f)
        {
            Debug.Log("Charge 1");
            currentState = BossState.Charge1;
            chargeTimer = 0f;
        }
    }

    void Charge1()
    {
        chargeTimer += Time.deltaTime;
        Vector2 currentPos = rb.position;
        Vector2 chargeEnd = (currentPos + new Vector2(-10, 0));

        Vector2 newPos = Vector2.Lerp(currentPos, chargeEnd, 0.9f * Time.deltaTime);

        
        rb.MovePosition(newPos);

        

        if (chargeTimer > 2.5f)
        {
            Debug.Log("Charge 2");
            currentState = BossState.Charge2;
            chargeTimer = 0f;
        }

    }

    void Charge2()
    {
        chargeTimer += Time.deltaTime;
        Vector2 currentPos = rb.position;
        Vector2 chargeEnd = (currentPos + new Vector2(10, 0));

        Vector2 newPos = Vector2.Lerp(currentPos, chargeEnd, 0.9f * Time.deltaTime);


        rb.MovePosition(newPos);


        if (chargeTimer > 3f)
        {
            Debug.Log("Charge 3");
            currentState = BossState.Charge3;
            chargeTimer = 0f;
        }
    }

    void Charge3()
    {
        chargeTimer += Time.deltaTime;
        Vector2 currentPos = rb.position;
        Vector2 chargeEnd = (currentPos + new Vector2(-10, 0));

        Vector2 newPos = Vector2.Lerp(currentPos, chargeEnd, 0.9f * Time.deltaTime);


        rb.MovePosition(newPos);



        if (chargeTimer > 2.75f)
        {
            Debug.Log("Idle");
            currentState = BossState.Idle;
            chargeTimer = 0f;
            idleTimer = 0f;
        }

    }

    void SummonAttack() 
    {
        if (!rangedPosSet)
        {
            rangedAttackPos = this.GetComponent<Rigidbody2D>().position + new Vector2(0, 3);
            rangedPosSet = true;
        }

        Vector2 currentPos = rb.position;
        Vector2 newPos = Vector2.Lerp(currentPos, rangedAttackPos, 8f * Time.deltaTime);
        rb.MovePosition(newPos);

        if (currentPos == newPos)
        {
            if (eyeballCount != 0)
            {
                Instantiate(eyeball, rb.position + new Vector2(-1.5f, 1f), Quaternion.identity); 
                Instantiate(eyeball, rb.position + new Vector2(1.5f, 1f), Quaternion.identity);
                eyeballCount -= 2;
            }
            currentState = BossState.Idle;
            Debug.Log(currentState);
            rangedPosSet = false;
            idleTimer = 0f;
            eyeballCount = 2;
        }
    }

    void Dead()
    {

    }

}
