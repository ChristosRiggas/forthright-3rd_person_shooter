using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float amount = 50;
    public Material[] material;

    private bool active;
    Renderer rend;
    private float waitTime = 45f;

    private void Start()
    {
        active = true;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health && health.currentHealth < health.maxHealth && active)
        {
            health.Heal(amount);
            DisablePickUp();
        }
    }

    private void DisablePickUp() //and change material from white to red bubble
    {
        active = false;
        rend.sharedMaterial = material[1]; //red
        StartCoroutine(WaitToEnablePickUp(waitTime));
    }

    IEnumerator WaitToEnablePickUp(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        active = true;
        rend.sharedMaterial = material[0]; //white
    }

    public void ResetPickUp()
    {
        StopAllCoroutines();
        active = true;
        rend.sharedMaterial = material[0];
    }
}
