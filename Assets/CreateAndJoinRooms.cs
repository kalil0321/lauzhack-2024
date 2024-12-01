using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
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
}
