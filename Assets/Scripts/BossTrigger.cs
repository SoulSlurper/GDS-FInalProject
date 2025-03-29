using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    public GameObject boss;
    bool hasSpawnedBoss = false;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasSpawnedBoss)
        {
            boss.SetActive(true);
            hasSpawnedBoss = true;
        }
    }
}
