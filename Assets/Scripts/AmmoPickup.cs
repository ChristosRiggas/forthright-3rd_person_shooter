using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public float amount = 50;
    public Material[] material;

    private bool active;
    Renderer rend;
    private float waitTime = 30f;

    private void Start()
    {
        active = true;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RaycastWeapon weapon = other.gameObject.transform.GetChild(4).GetChild(3).GetComponent<RaycastWeapon>();
            if (weapon && weapon.clipCount < 2 && active)
            {
                weapon.clipCount++;
                DisablePickUp();
            }
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
