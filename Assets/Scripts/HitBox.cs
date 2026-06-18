using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Health health;

    public void OnRaycastHit(RaycastWeapon weapon, Vector3 direction)
    {
        if (gameObject.tag == "head")
            health.TakeDamage(weapon.damage * 5, direction);
        else if (gameObject.tag == "Limb")
            health.TakeDamage(weapon.damage / 2, direction);
        else
            health.TakeDamage(weapon.damage, direction);
    }
}
