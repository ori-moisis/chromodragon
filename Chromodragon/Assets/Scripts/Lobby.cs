using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    enum Action
    {
        JoinRoom,
        CreateRoom,
        Quit,
        None
    }

	public int numPlayers;
    Action currentAction;
	public GameObject waitingForPlayersTextGO;
    Text waitingForPlayersText;
    

	// Use this for initialization
	void Start ()
	{
        this.waitingForPlayersText = this.waitingForPlayersTextGO.GetComponent<Text>();
        this.currentAction = Action.None;
		if (! PhotonNetwork.connected) {
			PhotonNetwork.autoCleanUpPlayerObjects = true;
			PhotonNetwork.automaticallySyncScene = true;
            this.DoAction();
		}
		Debug.Log ("Starting");
        InvokeRepeating("ShowNetworkStatus", 1, 1);
	}

	public void JoinRoom ()
	{
		Debug.Log ("Start game called");
        this.currentAction = Action.JoinRoom;
        this.DoAction();
	}

    public void CreateRoom()
    {
        Debug.Log("Create room called, in room=" + PhotonNetwork.inRoom);
        this.currentAction = Action.CreateRoom;
        this.DoAction();
    }

    public void LeaveRoom()
    {
        Debug.Log("Leave room called");
        this.currentAction = Action.None;
        PhotonNetwork.LeaveRoom();
    }

    public void ExitGame()
    {
        this.currentAction = Action.Quit;
        if (PhotonNetwork.connected)
        {
            Debug.Log("Disconnecting");
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.Log("Calling quit");
            Application.Quit();
        }
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected");
        if (this.currentAction == Action.Quit)
        {
            Debug.Log("Calling quit");
            Application.Quit();
        }
    }

    void DoAction()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }
        else if (PhotonNetwork.inRoom)
        {
            Debug.Log("Leaving current room");
            PhotonNetwork.LeaveRoom();
            this.ShowNetworkStatus();
        }
        else if (PhotonNetwork.insideLobby)
        {
            switch (this.currentAction)
            {
                case Action.CreateRoom:
                    {
                        RoomOptions opts = new RoomOptions();
                        opts.maxPlayers = 3;
                        opts.isOpen = true;
                        opts.isVisible = true;
                        PhotonNetwork.CreateRoom(null, opts, null);
                    }
                    break;
                case Action.JoinRoom:
                    {
                        PhotonNetwork.JoinRandomRoom();
                    }
                    break;
            }
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

	public void OnLeftRoom ()
	{
		Debug.Log ("left room");
        this.ShowNetworkStatus();
        this.DoAction();
	}

	void OnConnectedToMaster ()
	{
		Debug.Log ("Connected");
		PhotonNetwork.JoinLobby ();
	}

	void OnJoinedLobby ()
	{
		RoomInfo[] rooms = PhotonNetwork.GetRoomList ();
        this.ShowNetworkStatus();
		Debug.Log ("in lobby =" + PhotonNetwork.insideLobby + " with " + rooms.Length + " rooms");
        this.DoAction();
	}

	void OnPhotonRandomJoinFailed ()
	{
		Debug.Log ("Join random room failed");
        this.CreateRoom();
	}

	void OnPhotonCreateRoomFailed ()
	{
		Debug.Log ("Create room failed");
        waitingForPlayersText.text = "Failed to create room, try again later";
	}

	void OnCreatedRoom ()
	{
		Debug.Log ("created new room");
        this.currentAction = Action.None;
        this.ShowNetworkStatus();
	}

    void ShowNetworkStatus()
    {
        if (PhotonNetwork.inRoom)
        {
            waitingForPlayersText.text = "Waiting for other players: " + PhotonNetwork.playerList.Length + "/" + this.numPlayers;
        }
        else if (PhotonNetwork.insideLobby)
        {
            waitingForPlayersText.text = "In lobby with " + PhotonNetwork.GetRoomList().Length + " rooms";
        }
    }

	void OnJoinedRoom ()
	{
		Debug.Log ("in room with id " + PhotonNetwork.player.ID + " which has " + PhotonNetwork.playerList.Length + " players");
        this.currentAction = Action.None;
        this.ShowNetworkStatus();
		if (PhotonNetwork.playerList.Length == this.numPlayers) {
			this.EnoughPlayers ();
		}
	}

	void OnPhotonPlayerConnected ()
	{
		Debug.Log ("Player joinded, now there are " + PhotonNetwork.playerList.Length + " players");
        this.ShowNetworkStatus();
		if (PhotonNetwork.playerList.Length == this.numPlayers) {
			this.EnoughPlayers ();
		}
	}

	void EnoughPlayers ()
	{
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.room.visible = false;
            for (int i = 0; i < PhotonNetwork.playerList.Length; ++i)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("playerId", i);
                PhotonNetwork.playerList[i].SetCustomProperties(props);
            }
		}
        CancelInvoke();
        waitingForPlayersText.text = "Starting game";
		PhotonNetwork.LoadLevel ("Game");
	}

	void OnPhotonPlayerDisconnected ()
	{
		Debug.Log ("Player left");
        this.ShowNetworkStatus();
	}

	public void SinglePlayer ()
	{
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
		Application.LoadLevel ("Game");
	}
}
