using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public class ReloadWeapon : MonoBehaviour
{
    //private ThirdPersonActionsAsset playerActionAsset;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction reload;

    private PlayerManager users;
    private Health health;

    public CharacterLocomotion characterLocomotion;
    public Animator rigController;
    public WeaponAnimationEvents AnimationEvents;
    public RaycastWeapon weapon;
    public Transform leftHand;
    public AmmoWidget AmmoWidget;
    public bool reloading = false;

    GameObject magazineHand;

    private void Awake()
    {
        health = GetComponent<Health>();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

        users = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
        AmmoWidget = transform.GetChild(5).GetChild(0).GetChild(1).GetComponent<AmmoWidget>();

        if (characterLocomotion.userID == 1)
        {
            var ammoWidget = transform.GetChild(5).GetChild(0).GetChild(1).GetComponent<RectTransform>();
            var crossHair = transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            var healthbar = transform.GetChild(5).GetChild(0).GetChild(2).GetComponent<RectTransform>();

            ammoWidget.anchoredPosition += new Vector2(+400, 0);
            crossHair.anchoredPosition += new Vector2(+400, 0);
            healthbar.anchoredPosition += new Vector2(+400, 0);
        }
    }

    private void OnEnable()
    {
        reload = player.FindAction("Reload");
        reload.Enable();
        player.Enable();
    }

    private void OnDisable()
    {

        player.Disable();
    }

    void Start()
    {
        AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    // Update is called once per frame
    void Update()
    {

        if (((reload.WasPressedThisFrame() && weapon.clipCount > 0) || weapon.ShouldReload()) && reloading == false){ 
            weapon.StopFiring();
            reloading = true;
            rigController.SetTrigger("reload_weapon");
        }

        if(health.currentHealth <= 0.0f)
        {
            reloading = false;
            Destroy(magazineHand);
            weapon.magazine.SetActive(true);
        }

        AmmoWidget.Refresh(weapon.ammoCount, weapon.clipCount);
    }

    void OnAnimationEvent(string eventName){
        //Debug.Log(eventName);
        switch(eventName){
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;     
            case "attach_magazine":
                AttachMagazine();
                break;      
            case "reloaded_magazine":
                ReloadedMagazine();
                break;      
        }
    }

    void DetachMagazine(){
        magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }

    void DropMagazine(){
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);  
    }

    void RefillMagazine(){
        magazineHand.SetActive(true);
    }

    void AttachMagazine(){
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
        weapon.RefillAmmo();
        rigController.ResetTrigger("reload_weapon");
        AmmoWidget.Refresh(weapon.ammoCount, weapon.clipCount);
    }

    void ReloadedMagazine(){
        reloading = false;
    }

}
