using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeWeapon : Weapon
{
    [Header("Close Range Details")]
    [SerializeField] public Animator animator; //sprite and collider animator

    [Header("Attack Range Display")]
    [SerializeField] private bool showAttackRange = true;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackAngle = 90f;
    [SerializeField] private Color rangeColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private int fanSegments = 20;

    private LineRenderer lineRenderer;
    private SlimeKnightController playerController;

    #region Unity
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<SlimeKnightController>();
        }

        if (showAttackRange)
        {
            SetupRangeDisplay();
        }
    }

    void Update()
    {
        PerformAttack();

        if (showAttackRange && lineRenderer != null)
        {
            UpdateRangeDisplay();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("trigger detects: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Weapon") || collision.CompareTag("Item") || collision.CompareTag("BossTrigger")) return;

        MakeDamage(collision);
    }
    #endregion

    #region Attack Range Display
    private void SetupRangeDisplay()
    {
        GameObject rangeObject = new GameObject("AttackRangeDisplay");
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = Vector3.zero;

        lineRenderer = rangeObject.AddComponent<LineRenderer>();
        lineRenderer.material = CreateRangeMaterial();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = fanSegments + 2; // +2 for center point and closing the fan
    }

    private Material CreateRangeMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = rangeColor;
        return mat;
    }

    private void UpdateRangeDisplay()
    {
        bool shouldShow = playerController != null && playerController.IsFocusing() && isHeld;
        
        if (lineRenderer != null)
        {
            lineRenderer.enabled = shouldShow;

            if (shouldShow)
            {
                DrawFanShape();
            }
        }
    }

    private void DrawFanShape()
    {
        Vector3[] positions = new Vector3[fanSegments + 2];
        
        positions[0] = Vector3.zero;
        
        float halfAngle = attackAngle * 0.5f;
        float startAngle = -halfAngle;
        float angleStep = attackAngle / fanSegments;
        
        for (int i = 0; i <= fanSegments; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            float radians = currentAngle * Mathf.Deg2Rad;
            
            Vector3 point = new Vector3(
                Mathf.Cos(radians) * attackRange,
                Mathf.Sin(radians) * attackRange,
                0
            );
            
            positions[i + 1] = point;
        }
        
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
    #endregion

    #region Attack Details
    public override void Attack()
    {
        int randomAttack = Random.Range(1, 5);

        switch (randomAttack)
        {
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            case 3:
                animator.SetTrigger("Attack3");
                break;
            case 4:
                animator.SetTrigger("Attack4");
                break;
        }

        if (SoundManager.Instance != null)
        {
            Debug.Log("Attempting to play sword sound");
            SoundManager.Instance.PlaySwordSound();
        }
        else
        {
            Debug.LogWarning("SoungManager instance is null");
        }
    }
    #endregion

    #region Public Methods
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }

    public void SetAttackAngle(float angle)
    {
        attackAngle = angle;
    }

    public void SetShowAttackRange(bool show)
    {
        showAttackRange = show;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = show && playerController != null && playerController.IsFocusing() && isHeld;
        }
    }
    #endregion
}