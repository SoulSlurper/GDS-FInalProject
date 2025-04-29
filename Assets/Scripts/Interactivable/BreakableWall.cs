using UnityEngine;

public class BreakableWall : Status
{
    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Wall is destroyed");

            // Play wall break sound fully
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWallBreakSound(transform.position);
            }

            Destroy(gameObject);
        }
    }
}