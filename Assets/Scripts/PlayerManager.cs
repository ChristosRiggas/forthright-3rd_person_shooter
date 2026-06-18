using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerInput> players = new List<PlayerInput>();
    public List<Vector3> startingLocations = new List<Vector3>(); // empty now
    public List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;
    [SerializeField]
    private LevelMangaer levelManager;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        Time.timeScale = 0;
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        if (players.Count == 2)
        {
            levelManager.GetPlayers();
        }

        Transform playerParent = player.transform.parent;
        playerParent.position = startingPoints[players.Count - 1].position;
        startingLocations.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z));

        //convert layer mask (bit) to an integer 
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        //set the layer
        playerParent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
        //add the layer
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
    }
}
