using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ParallaxCameraFollow : MonoBehaviour
{
    public Transform target;          // The player or main object to follow
    public float smoothSpeed = 5f;
    public bool lockZ = true;         // Keep camera’s Z fixed

    Vector3 offset;

    void Start()
    {
        if (target != null)
            offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        if (lockZ)
            desiredPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}

