using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : Status
{
    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Enemy is destroyed");
            Instantiate(dropItem, gameObject.transform);
            Destroy(gameObject);
        }
    }
}
