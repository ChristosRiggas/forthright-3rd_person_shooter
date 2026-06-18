using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;

    public Score score;

    private void Start()
    {
        currentHealth = maxHealth;
        transform.position = new Vector3(Random.Range(-12.0f, 21.0f), Random.Range(2.5f, 7.5f), Random.Range(18.5f, 44.0f));
    }

    private void TargetFell(int userID)
    {
        //Debug.Log("Target Fell)");
        currentHealth = maxHealth;
        score.AddTargetScore(userID);
        score.scored = false;

        var position = new Vector3(Random.Range(-12.0f, 21.0f), Random.Range(2.5f, 7.5f), Random.Range(18.5f, 44.0f));
        transform.position = position;
        transform.rotation = Random.rotation;
    }

    public void OnRaycastHit(RaycastWeapon weapon)
    {
        currentHealth -= weapon.damage / 2;

        if (currentHealth <= 0)
        {
            var userID = weapon.gameObject.transform.root.GetChild(0).GetComponent<CharacterLocomotion>().userID;
            TargetFell(userID);
        }
    }

    
}
