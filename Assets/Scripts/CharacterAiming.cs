using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CharacterAiming : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction look;
    private InputAction attack;
    private int maxVerticalLookAngle = 60;
    public Vector2 delta;

    public float turnSpeed = 15;
    public float lookSpeed = 3f;

    public Transform cameraLookAt;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    RaycastWeapon weapon;

    public ReloadWeapon weaponReloading;

    private void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");
    }

    private void OnEnable()
    {
        look = player.FindAction("Look");
        attack = player.FindAction("Attack");
        player.Enable();
        look.Enable();
        attack.Enable();
    }

    private void OnDisable()
    {
        player.Disable();
        look.Disable();
        attack.Disable();
    }

    void Start()
    {
        weapon = GetComponentInChildren<RaycastWeapon>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xAxis.Value += delta.x * lookSpeed * Time.deltaTime;
        yAxis.Value += delta.y * lookSpeed * Time.deltaTime;

        if (yAxis.Value < -maxVerticalLookAngle)
            yAxis.Value = -maxVerticalLookAngle;
        if (yAxis.Value > maxVerticalLookAngle)
            yAxis.Value = maxVerticalLookAngle;

        cameraLookAt.eulerAngles = new Vector3(-yAxis.Value, xAxis.Value, 0);

        float yawCamera = cameraLookAt.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed + Time.fixedDeltaTime);
    }

    private void Update()
    {
        delta = look.ReadValue<Vector2>();

        if (attack.WasPressedThisFrame() && weaponReloading.reloading == false)
            weapon.StartFiring(); 

        if (attack.WasReleasedThisFrame())
            weapon.StopFiring();

        if (weapon.isFiring)
            weapon.UpdateFiring(Time.deltaTime);

        weapon.UpdateBullets(Time.deltaTime);
    }


}
