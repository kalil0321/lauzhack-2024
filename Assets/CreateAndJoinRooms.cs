using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMPro.TMP_InputField createInput;
    [SerializeField]
    public TMPro.TMP_InputField joinInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("House");
    }
}