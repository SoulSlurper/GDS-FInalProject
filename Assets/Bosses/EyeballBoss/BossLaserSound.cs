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
}


