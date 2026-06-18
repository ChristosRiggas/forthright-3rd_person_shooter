using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public GameObject[] players;

    public bool resurrect;
    public CharacterLocomotion charaterLocomotion;
    public RaycastWeapon raycastWeapon;
    //PlayerManager playerManager;
    LevelMangaer levelManager;
    public UIHealthBar healthBar;

    public Score score;
    //Ragdoll ragdoll;

    public bool died = false;

    private void Awake()
    {
        //playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelMangaer>();
        raycastWeapon = gameObject.transform.GetChild(4).GetChild(3).GetComponent<RaycastWeapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ragdoll = GetComponent<Ragdoll>();
        currentHealth = maxHealth;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
        }
    }

    private void Update()
    {
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        if (resurrect)
        {
            resurrect = false;
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;
        if(currentHealth <= 0.0f)
        {
            if (!died)
            {
                var sex = GetSex();
                levelManager.audioManager.PlaySFX(sex + 1);
                raycastWeapon.isFiring = false;
                raycastWeapon.HitSprite.gameObject.SetActive(false);
                died = true;
            }

            Die();
        }
    }

    private int GetSex()
    {
        var meshes = gameObject.transform.GetChild(1);

        if (meshes.GetChild(0).gameObject.activeSelf == true)
            return 0;
        else
            return 1;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();

        score.AddScoreToOther(charaterLocomotion.userID);
            //ragdoll.ActivateRagdoll();
        //StartCoroutine(DisablePlayerInputInLevelManager(2.5f));
        StartCoroutine(WaitForNextRound1(3.0f));
    }

    IEnumerator WaitForNextRound1(float seconds)
    {
        yield return new WaitForSeconds(seconds);
            //ragdoll.DeactivateRagdoll();
        levelManager.NextRound();
        died = false;
    }
}
