using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Runtime.ExceptionServices;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ToggleOnPlayerJoin : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject canvas;
    public PlayerManager users;
    public GameObject[] players;
    private int usersCount, lastUsersCount;

    public GameObject menu;
    public GameObject StartCanvas;
    public GameObject score;

    public AudioManager audioManager;

    //public TextMeshProUGUI
    public Queue<int> userTemp = new Queue<int>();  //public List<int> userTemp = new List<int>(); //private int userTemp; 
    private bool first = true;
    private void Awake()
    {
        //menu = GameObject.Find("Menu");
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void Update()
    {
        usersCount = users.players.Count;

        InputSystem.onDeviceChange +=
            (device, change) =>
            {  //if (this.gameObject != null) { 
               switch (change)
               {
                   case InputDeviceChange.Added:
                        //usersCount = users.players.Count;// New Device.
                        break;
                   case InputDeviceChange.Disconnected:
                        if(first == true) { lastUsersCount = usersCount; first = false;}//if (lastUsersCount == 2) { usersCount = 2; } Debug.Log("usersCount = " + usersCount);// Device got unplugged.
                        for(int i = 0; i < lastUsersCount; i++)
                        {
                            if (users.players[i].hasMissingRequiredDevices)
                            {
                                StartCanvas.transform.GetChild(i).gameObject.SetActive(false);
                                StartCanvas.transform.GetChild(i+2).gameObject.SetActive(true);
                                userTemp.Enqueue(i); lastUsersCount = usersCount; //Debug.Log("LasrUsersCount = " + lastUsersCount);//userTemp.Add(i); Debug.Log("Add 0" + userTemp[0]);//userTemp = i; 
                            }
                        }
                        break;
                   case InputDeviceChange.Reconnected:
                            var temp = userTemp.Dequeue(); //Debug.Log("temp = " + temp);//if (StartCanvas.transform.GetChild(userTemp.Dequeue()).gameObject != null) {  // Plugged back in.
                            StartCanvas.transform.GetChild(temp).gameObject.SetActive(true);
                            StartCanvas.transform.GetChild(temp + 2).gameObject.SetActive(false); //userTemp.RemoveAt(0);
                        //    }
                        break;
                   case InputDeviceChange.Removed:
                        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                        break;
                   default:
                        // See InputDeviceChange reference for other event types.
                        break;
               }//}
            };
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += PlayrJoined;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= PlayrJoined;
    }

    private void PlayrJoined(PlayerInput player)
    {
        StartCanvas.transform.GetChild(usersCount).gameObject.SetActive(true);
        StartCanvas.transform.GetChild(usersCount+2).gameObject.SetActive(false);
    }

    public void StartMatch()
    {
        //canvas.SetActive(true);

        if (usersCount == 2 && !users.players[0].hasMissingRequiredDevices && !users.players[1].hasMissingRequiredDevices) //all players joined with devices
        {
            if (audioManager == null)
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
            }
            audioManager.PlaySFX(0);

            players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject user in players)
            {
                if(user.GetComponent<CharacterLocomotion>().userID == 0)
                {
                    var userCamera = user.transform.parent.GetChild(1).gameObject;
                    userCamera.GetComponent<AudioListener>().enabled = true;
                }

                user.transform.GetChild(5).gameObject.SetActive(true);

                if (GameObject.Find("PracticeSettings"))
                {
                    user.transform.GetChild(5).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
            }

            menu.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            StartCanvas.transform.gameObject.SetActive(false);
            score.SetActive(true);

            //clear selected UI object
            EventSystem.current.SetSelectedGameObject(null); 

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;

            this.gameObject.SetActive(false);
        }
    }
}
