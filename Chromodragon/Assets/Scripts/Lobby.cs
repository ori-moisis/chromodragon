using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour {

    public int numPlayers;

	// Use this for initialization
	void Start () {
        PhotonNetwork.autoCleanUpPlayerObjects = true;
        PhotonNetwork.ConnectUsingSettings("v0.1");
        Debug.Log("Starting");
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
        PhotonNetwork.CreateRoom(null);
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
        Debug.Log("here");
        Application.LoadLevel("Game");
    }

    void OnPhotonPlayerDisconnected()
    {
        Debug.Log("Player left");
    }

	// Update is called once per frame
	void Update () {
	
	}
}
