using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBarrier : MonoBehaviour
{
    private Collider2D barrierCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        barrierCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DeactivateBarrier()
    {
        if (barrierCollider) barrierCollider.enabled = false;
        if (spriteRenderer) spriteRenderer.enabled = false;
        Debug.Log("Barrier deactivated!");
    }
}