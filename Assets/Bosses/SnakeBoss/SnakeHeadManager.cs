using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHeadManager : MonoBehaviour
{
    //Snake Head Variables
    Rigidbody2D rb;
    BoxCollider2D bc;
    Vector2 initialPos;


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
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        Vector2 targetPosition = playerPos + new Vector2(10,0);
        rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, 2f * Time.deltaTime));
    }

    public void biteAttack()
    {
    }

    public void shootAttack()
    {
    }
}
