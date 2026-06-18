using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Animator animator;
    CharacterAiming characterAiming;
    CharacterController characterController;
    CharacterLocomotion characterLocomotion;
    Health health;
    Transform rigLayers;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        characterAiming = GetComponent<CharacterAiming>();
        characterController = GetComponent<CharacterController>();
        characterLocomotion = GetComponent<CharacterLocomotion>();
        rigLayers = gameObject.transform.GetChild(4);

        health = gameObject.GetComponent<Health>();
        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach(var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }

        StartCoroutine(WaitSomeTime(0.0f));
    }

    public void ActivateRagdoll()
    { 
        if(health.currentHealth <= 0f)
        {
            foreach (var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = false;
            }
            animator.enabled = false;
            rigLayers.gameObject.SetActive(false);
        }
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);
        characterAiming.enabled = false;
        characterController.enabled = false;
        characterLocomotion.enabled = false;
    }

    IEnumerator WaitSomeTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        animator.enabled = true;
        characterAiming.enabled = true;
        characterController.enabled = true;
        characterLocomotion.enabled = true;
        rigLayers.gameObject.SetActive(true);
    }
}
