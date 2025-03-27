using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    void Update()
    {
        Vector2 direction = FacePointerPosition();

        transform.right = direction;

        Vector2 scale = transform.localScale;
        if ((direction.x < 0 && scale.y > 0) || (direction.x > 0 && scale.y < 0))
        {
            scale.y *= -1;
        }
        transform.localScale = scale;
    }

    private Vector2 FacePointerPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (mousePosition - (Vector2)transform.position).normalized;
    }
}
