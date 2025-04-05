using UnityEngine;

public class BreakableWall : Status
{
    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Wall is destroyed");
            Destroy(gameObject);
        }
    }
}