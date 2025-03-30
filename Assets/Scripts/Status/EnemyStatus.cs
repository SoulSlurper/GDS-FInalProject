using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : Status
{
    void Update()
    {
        health.RegainingAmount(Time.deltaTime);

        if (noHealth)
        {
            Destroy(gameObject);
        }
    }

    public override StatusType GetStatusType() { return StatusType.Enemy; }
}
