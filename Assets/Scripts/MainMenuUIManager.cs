using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject mainFirstButton, playFirstButton, optionsFirstButton, practiceFirstButton, versusFirstButton;
    public GameObject practiceButton, versusButton;
    public AudioManager audioManager;
    private int currentChild;

    private bool hoverPractice, hoverVersus;

    private void Update()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        }

        if (!hoverPractice)
        {
            if (EventSystem.current.currentSelectedGameObject == practiceButton)
                gameObject.transform.GetChild(4).GetChild(3).gameObject.SetActive(true);
            else
                gameObject.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
        }

        if (!hoverVersus)
        {
            if (EventSystem.current.currentSelectedGameObject == versusButton)
                gameObject.transform.GetChild(4).GetChild(4).gameObject.SetActive(true);
            else
                gameObject.transform.GetChild(4).GetChild(4).gameObject.SetActive(false);
        }

        if (GameObject.Find("MainMenu"))
        {
            gameObject.transform.GetChild(7).gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.GetChild(7).gameObject.SetActive(false);
        }

    }

    private void HideOtherMenus(int current)
    {
        for (int i = 2; i < transform.childCount; i++)
        {
            if (i != current)
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            else
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void MainMenu()
    {
        currentChild = 2;
        HideOtherMenus(currentChild);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainFirstButton);
        audioManager.PlaySFX(0);
    }

    public void OptionsMenu()
    {
        currentChild = 3;
        HideOtherMenus(currentChild);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        audioManager.PlaySFX(0);
    }

    public void PlayMenu()
    {
        currentChild = 4;
        HideOtherMenus(currentChild);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playFirstButton);
        audioManager.PlaySFX(0);
    }

    public void PracticeMenu()
    {
        currentChild = 5;
        HideOtherMenus(currentChild);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(practiceFirstButton);
        audioManager.PlaySFX(0);
    }

    public void VersusMenu()
    {
        currentChild = 6;
        HideOtherMenus(currentChild);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(versusFirstButton);
        audioManager.PlaySFX(0);
    }

    public void PointerEnterPractice()
    {
        gameObject.transform.GetChild(4).GetChild(3).gameObject.SetActive(true);
        hoverPractice = true;
    }

    public void PointerExitPractice()
    {
        gameObject.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
        hoverPractice = false;
    }

    public void PointerEnterVersus()
    {
        gameObject.transform.GetChild(4).GetChild(4).gameObject.SetActive(true);
        hoverVersus = true;
    }

    public void PointerExitVersus()
    {
        gameObject.transform.GetChild(4).GetChild(4).gameObject.SetActive(false);
        hoverVersus = false;
    }

    public void PracticeMap()
    {
        SceneManager.LoadScene(2);
    }

    public void TownMap()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitToDesctop()
    {
        Application.Quit();
    }
}
