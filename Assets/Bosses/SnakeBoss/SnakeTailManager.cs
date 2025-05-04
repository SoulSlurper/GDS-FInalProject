using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class SnakeTailManager : MonoBehaviour
{
    //Tail Components
    Rigidbody2D rb;
    BoxCollider2D bc;
    Vector2 initialPos;

    //Tail Attack Variables
    float tailTimer = 0f;
    int tailAttacks = 0;
    public bool tailAttackDone = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        initialPos = rb.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tailAttack(Vector2 playerPos) 
    {
        tailTimer += Time.deltaTime;

        //set the down position to an actual position


        if (tailAttacks < 3)
        {
            bc.isTrigger = true;

            if (tailTimer < 1.5f)
            {
                Vector2 targetPosition = initialPos + new Vector2(0,-4);
                rb.position = Vector2.Lerp(rb.position, targetPosition, 2.4f * Time.deltaTime);
            }
            else if (tailTimer < 5f)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                Vector2 targetPosition = new Vector2(playerPos.x, 0);
                rb.MovePosition(Vector2.Lerp(rb.position, playerPos, 2f * Time.deltaTime));
            }
            else if (tailTimer < 5.25f)
            {
                rb.constraints = RigidbodyConstraints2D.None;
                Vector2 targetPosition = rb.position + Vector2.up;
                rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, 10f * Time.deltaTime));
                
            }
            else if (tailTimer < 5.5f)
            {

                Vector2 targetPosition = new Vector2(0,initialPos.y) + new Vector2(rb.position.x, -4);
                rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, 9.175f * Time.deltaTime));
            }
            else
            {
                tailTimer = 2.5f;
                tailAttacks++;
            }
        }
        else
        {
            
          rb.constraints = RigidbodyConstraints2D.FreezePositionY;
          rb.MovePosition(Vector2.Lerp(rb.position, initialPos, 2f * Time.deltaTime));
            
        }

        if (tailTimer > 5.6f)
        {
            tailAttackDone = true;
            tailTimer = 0f;
            tailAttacks = 0;
        }
        
    }

    public void biteAttack()
    {
    }

    public void shootAttack()
    {
    }
}
