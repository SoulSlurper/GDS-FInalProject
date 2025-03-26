using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLongRange : Weapon
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AttackEnemy();
        }
    }
}
