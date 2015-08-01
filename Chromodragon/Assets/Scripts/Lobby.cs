using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{

	public int numPlayers;
	bool shouldJoinRoom;
	public GameObject waitingForPlayersText;
    

	// Use this for initialization
	void Start ()
	{
		shouldJoinRoom = false;
		if (! PhotonNetwork.connected) {
			PhotonNetwork.autoCleanUpPlayerObjects = true;
			PhotonNetwork.automaticallySyncScene = true;
		}
		Debug.Log ("Starting");
	}

	public void StartGame ()
	{
		Debug.Log ("Start game called");
		this.shouldJoinRoom = true;
		if (! PhotonNetwork.connected) {
			PhotonNetwork.ConnectUsingSettings ("v0.1");
		} else if (! PhotonNetwork.insideLobby) {
			PhotonNetwork.JoinLobby ();
		} else {
			PhotonNetwork.JoinRandomRoom ();
		}
		waitingForPlayersText.GetComponent<Text> ().text = "Waiting for other players";
	}

	public void OnLeftRoom ()
	{
		Debug.Log ("left room");
	}

	void OnConnectedToMaster ()
	{
		Debug.Log ("Connected");
		PhotonNetwork.JoinLobby ();
	}

	void OnJoinedLobby ()
	{
		RoomInfo[] rooms = PhotonNetwork.GetRoomList ();
		Debug.Log ("in lobby =" + PhotonNetwork.insideLobby + " with " + rooms.Length + " rooms");
		if (this.shouldJoinRoom) {
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	void OnPhotonRandomJoinFailed ()
	{
		Debug.Log ("Join random room failed");
		RoomOptions opts = new RoomOptions ();
		opts.maxPlayers = 3;
		opts.isOpen = true;
		opts.isVisible = true;
		PhotonNetwork.CreateRoom (null, opts, null);
	}

	void OnPhotonCreateRoomFailed ()
	{
		Debug.Log ("Create room failed");
	}

	void OnCreatedRoom ()
	{
		Debug.Log ("created new room");
	}

	void OnJoinedRoom ()
	{
		Debug.Log ("in room with id " + PhotonNetwork.player.ID + " which has " + PhotonNetwork.playerList.Length + " players");
		if (PhotonNetwork.playerList.Length == this.numPlayers) {
			this.EnoughPlayers ();
		}
	}

	void OnPhotonPlayerConnected ()
	{
		Debug.Log ("Player joinded, now there are " + PhotonNetwork.playerList.Length + " players");
		if (PhotonNetwork.playerList.Length == this.numPlayers) {
			this.EnoughPlayers ();
		}
	}

	void EnoughPlayers ()
	{
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.room.visible = false;
		}
		PhotonNetwork.LoadLevel ("Game");
	}

	void OnPhotonPlayerDisconnected ()
	{
		Debug.Log ("Player left");
	}

	public void SinglePlayer ()
	{
		Application.LoadLevel ("Game");
	}
}
