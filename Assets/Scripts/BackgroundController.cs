using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        float camPosition = cam.transform.position.x;

        // Wrap the background properly
        if (camPosition > startPos + length)
        {
            startPos += length;
        }
        else if (camPosition < startPos - length)
        {
            startPos -= length;
        }
    }
}

