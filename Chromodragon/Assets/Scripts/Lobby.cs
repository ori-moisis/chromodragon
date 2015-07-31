using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {

    public int numPlayers;
    public GameObject waitingForPlayersText;
    

	// Use this for initialization
	void Start () {
        PhotonNetwork.autoCleanUpPlayerObjects = true;
        PhotonNetwork.automaticallySyncScene = true;
        Debug.Log("Starting");
	}

    public void StartGame()
    {
        PhotonNetwork.ConnectUsingSettings("v0.1");
        waitingForPlayersText.GetComponent<Text>().text = "Waiting for other players";
    }

    void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        Debug.Log("in lobby =" + PhotonNetwork.insideLobby + " with " + rooms.Length + " rooms");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Join random room failed");
        RoomOptions opts = new RoomOptions();
        opts.maxPlayers = 3;
        opts.isOpen = true;
        opts.isVisible = true;
        PhotonNetwork.CreateRoom(null, opts, null);
    }

    void OnPhotonCreateRoomFailed()
    {
        Debug.Log("Create room failed");
    }

    void OnCreatedRoom()
    {
        Debug.Log("created new room");
    }

    void OnJoinedRoom()
    {
        Debug.Log("in room with id " + PhotonNetwork.player.ID + " which has " + PhotonNetwork.playerList.Length + " players");
        if (PhotonNetwork.playerList.Length == this.numPlayers)
        {
            this.EnoughPlayers();
        }
    }

    void OnPhotonPlayerConnected()
    {
        Debug.Log("Player joinded, now there are " + PhotonNetwork.playerList.Length + " players");
        if (PhotonNetwork.playerList.Length == this.numPlayers)
        {
            this.EnoughPlayers();
        }
    }

    void EnoughPlayers()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    void OnPhotonPlayerDisconnected()
    {
        Debug.Log("Player left");
    }
}
