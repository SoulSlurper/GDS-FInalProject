using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHp : Status
{
    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Boss is destroyed");
            Destroy(gameObject);
        }
    }
}
