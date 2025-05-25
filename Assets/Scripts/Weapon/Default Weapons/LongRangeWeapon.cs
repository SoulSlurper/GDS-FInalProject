using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField][Range(0f, 1f)] private float _minProjectileCost = 1f;

    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchLocation; //where the projectile will appear
    [SerializeField] public Animator animator;

    [Header("Projectile Details")]
    [SerializeField] private float _launchForce = 1f;
    [SerializeField] private bool _usesGravity = false;
    [SerializeField] private float _projectileCost = 0f;
    [SerializeField] private bool _healthItemReturnsProjectileCost = true;
    [SerializeField][Range(0f, 1f)] private float _stopGapHealth = 0f; //stops making projectiles when the current health reaches at a certain point

    [Header("Trajectory Display")]
    [SerializeField] private bool showTrajectory = true;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [SerializeField] private Color trajectoryColor = new Color(0f, 1f, 0f, 0.8f);
    [SerializeField] private float maxTrajectoryDistance = 10f;

    private float _realProjectileCost;
    private LineRenderer trajectoryLineRenderer;
    private SlimeKnightController playerController;

    #region Getter and Setters
    public float launchForce
    {
        get { return _launchForce; }
        private set { _launchForce = value; }
    }

    public bool usesGravity
    {
        get { return _usesGravity; }
        private set { _usesGravity = value; }
    }

    public float projectileCost
    {
        get { return _projectileCost; }
        private set { _projectileCost = value; }
    }

    public float minProjectileCost
    {
        get { return _minProjectileCost; }
        private set { _minProjectileCost = value; }
    }

    public bool healthItemReturnsProjectileCost
    {
        get { return _healthItemReturnsProjectileCost; }
        private set { _healthItemReturnsProjectileCost = value; }
    }

    public float realProjectileCost
    {
        get { return _realProjectileCost; }
        private set { _realProjectileCost = value; }
    }

    public float stopGapHealth
    {
        get { return _stopGapHealth; }
        private set { _stopGapHealth = value; }
    }
    #endregion

    #region Unity
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<SlimeKnightController>();
        }

        if (showTrajectory)
        {
            SetupTrajectoryDisplay();
        }
    }

    void Update()
    {
        PerformAttack();

        if (showTrajectory && trajectoryLineRenderer != null)
        {
            UpdateTrajectoryDisplay();
        }
    }
    #endregion

    #region Trajectory Display
    private void SetupTrajectoryDisplay()
    {
        GameObject trajectoryObject = new GameObject("TrajectoryDisplay");
        trajectoryObject.transform.SetParent(transform);
        trajectoryObject.transform.localPosition = Vector3.zero;

        trajectoryLineRenderer = trajectoryObject.AddComponent<LineRenderer>();
        trajectoryLineRenderer.material = CreateTrajectoryMaterial();
        trajectoryLineRenderer.startWidth = 0.03f;
        trajectoryLineRenderer.endWidth = 0.03f;
        trajectoryLineRenderer.useWorldSpace = true;
        trajectoryLineRenderer.positionCount = trajectoryPoints;
    }

    private Material CreateTrajectoryMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = trajectoryColor;
        return mat;
    }

    private void UpdateTrajectoryDisplay()
    {
        bool shouldShow = playerController != null && playerController.IsFocusing() && isHeld;
        
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.enabled = shouldShow;

            if (shouldShow)
            {
                DrawTrajectory();
            }
        }
    }

    private void DrawTrajectory()
    {
        if (launchLocation == null) return;

        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = launchLocation.position;
        Vector3 startVelocity = launchLocation.right * launchForce;

        if (usesGravity)
        {
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryTimeStep;
                Vector3 point = startPos + startVelocity * time;
                point.y += 0.5f * Physics2D.gravity.y * time * time;

                points[i] = point;

                if (Vector3.Distance(startPos, point) > maxTrajectoryDistance)
                {
                    System.Array.Resize(ref points, i + 1);
                    trajectoryLineRenderer.positionCount = i + 1;
                    break;
                }
            }
        }
        else
        {
            Vector3 direction = launchLocation.right;
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float distance = (i * trajectoryTimeStep * launchForce);
                if (distance > maxTrajectoryDistance)
                {
                    System.Array.Resize(ref points, i);
                    trajectoryLineRenderer.positionCount = i;
                    break;
                }
                points[i] = startPos + direction * distance;
            }
        }

        trajectoryLineRenderer.SetPositions(points);
    }
    #endregion

    #region Long Range Details
    public void IncreaseLaunchForce(float amount) { launchForce += amount; }

    public void DecreaseLaunchForce(float amount) 
    { 
        launchForce -= amount; 
        if (launchForce < 0f) launchForce = 0f;
    }

    public void SetLaunchForce(float launchForce) 
    { 
        if (launchForce < 0f) this.launchForce = 0f;
        else this.launchForce = launchForce;
    }

    public void SetUseGravity(bool usesGravity) 
    {
        this.usesGravity = usesGravity;
    }

    public void IncreaseProjectileCost(float amount) { projectileCost += amount; }
    
    public void DecreaseProjectileCost(float amount) 
    { 
        projectileCost -= amount;
        if (projectileCost < 0f) projectileCost = 0f;
    }

    public void SetProjectileCost(float projectileCost) 
    { 
        if (projectileCost < 0f) this.projectileCost = 0f;
        else this.projectileCost = projectileCost;
    }

    public void SetStopGapHealth(float stopGapHealth) 
    { 
        if (stopGapHealth >= 0f && stopGapHealth <= 1f) this.stopGapHealth = stopGapHealth;
        else if (stopGapHealth > 1f) this.stopGapHealth = 1f;
        else this.stopGapHealth = 0f;
    }

    public override void SetRealAmounts()
    {
        base.SetRealAmounts();

        realProjectileCost = GetRealAmount(projectileCost, minProjectileCost);
        //Debug.Log("realProjectileCost: " + realProjectileCost);
    }
    #endregion

    #region Attack Details
    public override void Attack()
    {
        //whether the weapon can be used or not
        bool useWeapon = weaponUser.currentHealthPercentage > stopGapHealth || projectileCost == 0f;

        if (useWeapon && projectileCost > 0f) weaponUser.TakeDamage(realProjectileCost);

        if (!weaponUser.noHealth && useWeapon)
        {
            SpawnProjectile();

            animator.SetTrigger("Fire");

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayShootSound();
            }
            else
            {
                Debug.LogWarning("SoundManager instance is null! Shoot sound won't play.");
            }
        }
    }

    private void SpawnProjectile()
    {
        //instantiates as a child object in the LaunchLocation Transform to be accurately in its location when the weapon is flipped on the left side
        GameObject projectileObject = Instantiate(projectile.gameObject, launchLocation.position, launchLocation.rotation);
        ProjectileWeapon wDetails = projectileObject.GetComponent<ProjectileWeapon>();

        //based on the gameobject containing the WeaponAtHand.cs, where the arrow is not yet flipped (the arrow is slightly not in the center)
        if (transform.parent.GetComponent<WeaponAtHand>())
        {
            if (transform.parent.transform.localScale.y < 0f)
            {
                Vector3 scale = projectileObject.transform.localScale;
                scale.y *= -1;
                projectileObject.transform.localScale = scale;
            }
        }

        wDetails.SetWeaponUser(weaponUser);
        wDetails.SetUsesGravity(usesGravity);
        wDetails.SetLaunchForce(launchForce);

        if (healthItemReturnsProjectileCost)
        {
            wDetails.dropItem.GetComponent<HealthItem>().SetHealthAmount(realProjectileCost);
        }

        // Use realDamage instead of damage to apply focusing bonus to projectiles
        wDetails.SetDamage(realDamage);

        wDetails.Attack();
    }
    #endregion

    #region Public Methods
    public void SetShowTrajectory(bool show)
    {
        showTrajectory = show;
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.enabled = show && playerController != null && playerController.IsFocusing() && isHeld;
        }
    }

    public void SetTrajectoryPoints(int points)
    {
        trajectoryPoints = points;
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = points;
        }
    }

    public void SetMaxTrajectoryDistance(float distance)
    {
        maxTrajectoryDistance = distance;
    }
    #endregion
}