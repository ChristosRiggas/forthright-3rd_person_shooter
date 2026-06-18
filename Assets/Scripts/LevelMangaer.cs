using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LevelMangaer : MonoBehaviour
{

    public PlayerManager playerManager;
    public GameObject[] players;
    public bool GameisPaused = false;
    public GameObject mainCamera;
    public GameObject pauseFirstButton, optionsFirstButton;
    public Score score;

    private InputActionMap playerActionMap;
    public GameObject menu;
    public GameObject pauseMenu;
    private GameObject optionsMenu;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        pauseMenu = menu.transform.GetChild(0).GetChild(0).gameObject;
        optionsMenu = menu.transform.GetChild(0).GetChild(2).gameObject;
    }

    void Update()
    {
        if (players.Length == 2 && mainCamera.activeSelf == false) TestIfDeviceStateChange();
        if(audioManager == null)
        {
            FindAudioManager();
        }

        if (players.Length == 2 && mainCamera.activeSelf == false)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            if (players[0].GetComponent<Health>().currentHealth <= 0f || players[1].GetComponent<Health>().currentHealth <= 0f)
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<Ragdoll>().ActivateRagdoll();
                }
            }
        }
    }

    private void FindAudioManager()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    /// <summary>
    /// //////////////////// UI and Input System
    /// </summary>


    private void TestIfDeviceStateChange()
    {
        InputSystem.onDeviceChange +=
           (device, change) =>
           {
               switch (change)
               {
                   case InputDeviceChange.Disconnected:
                       // Device got unplugged.
                       if (players.Length == 2 && mainCamera.activeSelf == false)
                       {   if(mainCamera.activeSelf == false) { 
                           PauseGame();
                           pauseMenu.transform.GetChild(4).gameObject.SetActive(true);
                           audioManager.PlaySFX(4);
                           audioManager.musicSlider.value = 0f;
                           audioManager.UpdateMusic(); }
                       }
                       break;
                   case InputDeviceChange.Reconnected:
                       if (TestIfAllDevicesAreConntected() == true && mainCamera.activeSelf == false)//!playerManager.players[0].hasMissingRequiredDevices && !playerManager.players[1].hasMissingRequiredDevices) // Plugged back in
                       pauseMenu.transform.GetChild(4).gameObject.SetActive(false);
                       break;
                   default:
                       // See InputDeviceChange reference for other event types.
                       break;
               }
           };
    }

    public void GetPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void PauseGame()
    { 
        if(optionsMenu.activeSelf == false)
        {
            audioManager.PlaySFX(0);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);

            //Disable InputMapAsset Player
            players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject user in players)
            {
                var cLoco = user.transform.GetComponentInChildren<CharacterLocomotion>();
                cLoco.player.Disable();
            }

            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (players.Length == 2 && mainCamera.activeSelf == false)
        {
            audioManager.PlaySFX(0);

            if (!playerManager.players[0].hasMissingRequiredDevices && !playerManager.players[1].hasMissingRequiredDevices)
            {
                //Eneble InputMapAsset Player
                players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject user in players)
                {
                    var cLoco = user.transform.GetComponentInChildren<CharacterLocomotion>();
                    cLoco.player.Enable();
                }

                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.SetActive(false);
            }
        }
    }

    public void OptionsMenu()
    {
        audioManager.PlaySFX(0);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);

        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void OptionsBack()
    {
        audioManager.PlaySFX(0);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        audioManager.PlaySFX(0);

        SceneManager.LoadScene(0);
    }

/// <summary>
/// ///////////////////// Level
/// </summary>
/// 

    public void NextRound()
    {
        score.scored = false;
        RespawnPlayers();
        EnablePickUps();
    }

    private void EnablePickUps()
    {
        GameObject pickUps = GameObject.Find("PickUps");

        for (int i = 0; i < pickUps.transform.childCount; i++)
        {
            var pickUp = pickUps.transform.GetChild(i);
            if (pickUp.GetComponent<AmmoPickup>())
            {
                var ammo = pickUp.GetComponent<AmmoPickup>();
                ammo.ResetPickUp();
            }
            else if(pickUp.GetComponent<HealthPickup>())
            {
                var health = pickUp.GetComponent<HealthPickup>();
                health.ResetPickUp();
            }
        }
    }

    private void RespawnPlayers()
    {
        playerManager.players[0].transform.position = playerManager.startingPoints[0].position;
        playerManager.players[1].transform.position = playerManager.startingPoints[1].position;

        foreach (GameObject player in players)
        {
            var raycastWeapon = player.transform.GetChild(4).GetChild(3).GetComponent<RaycastWeapon>();   

            player.GetComponent<Health>().resurrect = true;
            raycastWeapon.ammoCount = raycastWeapon.clipSize;
            raycastWeapon.clipCount = 2;
            player.GetComponent<Ragdoll>().DeactivateRagdoll();

            raycastWeapon.isFiring = false;
            raycastWeapon.HitSprite.gameObject.SetActive(false);
        }
    }

    private bool TestIfAllDevicesAreConntected()
    {
        var lost = false;
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (playerManager.players[i].hasMissingRequiredDevices)
            {
                lost = false;
            }
            else
            {
                lost = true;
            }
        }
        if(lost) return true; else
        {
            return false;
        }
    }
}
