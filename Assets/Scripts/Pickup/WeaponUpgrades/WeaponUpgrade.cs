using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : PickupItem
{
    [Header("Weapon Upgrade")]
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _increaseDamageBy;
    [SerializeField] private float _improveCostsBy;

    // Getter and Setters // // // //
    public WeaponType type
    {
        get { return _type; }
        private set { _type = value; }
    }

    public float increaseDamageBy
    {
        get { return _increaseDamageBy; }
        private set { _increaseDamageBy = value; }
    }

    public float improveCostsBy
    {
        get { return _improveCostsBy; }
        private set { _improveCostsBy = value; }
    }

    // Functions // // // //
    public void ResetCameraSize()
    {
        Camera mainCamera = Camera.main;
        if (!mainCamera.orthographic.Equals(3f)) 
        {
            Debug.Log("Reset Camera Size");
            Camera.main.orthographicSize = 3f;
        }
    }
}
