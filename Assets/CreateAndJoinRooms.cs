using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public bool host;
    private bool isReadyToCreateOrJoinRoom = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        isReadyToCreateOrJoinRoom = true;
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("1234");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("1234");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main");
    }

    void Update()
    {
        if (isReadyToCreateOrJoinRoom)
        {
            if (host)
            {
                CreateRoom();
            }
            else
            {
                JoinRoom();
            }
            isReadyToCreateOrJoinRoom = false; // Ensure we only try to create or join the room once
        }
    }
}
