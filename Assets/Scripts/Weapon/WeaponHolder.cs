using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;

    void Start()
    {
        SelectWeapon();
    }

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

    private void SelectWeapon()
    {
        foreach (Transform weapon in transform)
        {
            Debug.Log(weapon.name);
            if (weapon.GetComponent<Weapon>().GetWeaponType() == weaponType)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    private Vector2 FacePointerPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (mousePosition - (Vector2)transform.position).normalized;
    }
}
