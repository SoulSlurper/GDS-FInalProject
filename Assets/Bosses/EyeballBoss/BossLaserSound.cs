using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaserSound : MonoBehaviour
{
    public void PlayLaserShootSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayEnemyLaserShootSound(transform.position);
        }
    }
    // TODO: there is still no sound when boss shoots laser
}


