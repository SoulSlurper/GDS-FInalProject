using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private Status player;
    [SerializeField] private Color _activeColor = Color.black;

    private Color _initialColor;
    private bool _isActive;
    private Vector2 _position;
    public Animator animator;

    private SpriteRenderer spriteRenderer;

    // Getters and Setters // // // //


    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public Vector2 position
    {
        get { return _position; }
        set { _position = value; }
    }

    // Unity // // // //
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        if (isActive)
        {
            animator.SetBool("SavePointOn", true);
        }

        else
        {
            animator.SetBool("SavePointOn", false);
        }
    }
}
