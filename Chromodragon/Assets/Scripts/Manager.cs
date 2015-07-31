﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

	public static Manager instance;

	public Creature CreaturePrefab;
	public GameObject hexTilePrefab;
	public Vector3[] cameraPositions;
	public GameObject creatures;
	public GameObject tiles;

	public Slider greenScoreSlider;
	public Slider purpleScoreSlider;
	public Slider orangeScoreSlider;

    public Image[] turnImages;
    public GameColors[] targetColors;

	int numGreen = 0;
	int numPurple = 0;
	int numOrange = 0;

	int numCreatures = 0;

	public int hexRadius = 2;  // CHANGE IN GUI, here doesn't matter :(
	public int currentTurn = 0;

	// Map hexagon-cube coordinates to creatures:
	Creature[, ,] coordToCreature;

	// Map creaturee to hexagon-cube coordinates:
	Dictionary<Creature, int[]> creatureToCoord = new Dictionary<Creature, int[]> ();

	// Use this for initialization
	void Awake ()
	{
		if (instance == null) {
			instance = this;
		}
		initWorld (hexRadius);
		if (PhotonNetwork.inRoom) {
			Camera.main.transform.position = cameraPositions [PhotonNetwork.player.ID - 1];
			Camera.main.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.position, new Vector3 (0, 1, 0));
            setCurrentTurnImgColor(true);
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
	    
	}


	public List<Creature> getNeighbours (Creature creature)
	{
		var neighbours = new List<Creature> ();
		int[] coord = creatureToCoord [creature];

		if (coord [0] > 0) {
			neighbours.Add (coordToCreature [coord [0] - 1, coord [1], coord [2]]);
		}
		if (coord [0] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0] + 1, coord [1], coord [2]]);
		}

		if (coord [1] > 0) {
			neighbours.Add (coordToCreature [coord [0], coord [1] - 1, coord [2]]);
		}
		if (coord [1] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0], coord [1] + 1, coord [2]]);
		}

		if (coord [2] > 0) {
			neighbours.Add (coordToCreature [coord [0], coord [1], coord [2] - 1]);
		}
		if (coord [2] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0], coord [1], coord [2] + 1]);
		}

		return neighbours;
	}

    int currentTurnIndex()
    {
        return (((currentTurn - PhotonNetwork.player.ID + 1) % 3) + 3) % 3;
    }

    void setCurrentTurnImgColor(bool isSet)
    {
        if (PhotonNetwork.inRoom)
        {
            Color newColor = ColorsManager.colorMap[this.targetColors[currentTurn]];
            newColor.a = isSet ? 10 : 0;
            this.turnImages[currentTurnIndex()].color = newColor;
        }
    }

    public void updateTurn()
    {
        setCurrentTurnImgColor(false);
        currentTurn = (currentTurn + 1) % PhotonNetwork.playerList.Length;
        setCurrentTurnImgColor(true);
    }

    public bool isMyTurn()
    {
        return (currentTurn + 1) == PhotonNetwork.player.ID;
    }

	public void updateScore (GameColors prevColor, GameColors newColor)
	{
		if (prevColor == GameColors.Green) {
			numGreen--;
		} else if (prevColor == GameColors.Purple) {
			numPurple--;
		} else if (prevColor == GameColors.Orange) {
			numOrange--;
		}

		if (newColor == GameColors.Green) {
			numGreen++;
		} else if (newColor == GameColors.Purple) {
			numPurple++;
		} else if (newColor == GameColors.Orange) {
			numOrange++;
		}

		greenScoreSlider.value = (float)(numGreen) / numCreatures;
		purpleScoreSlider.value = (float)(numPurple) / numCreatures;
		orangeScoreSlider.value = (float)(numOrange) / numCreatures;

        Debug.Log(greenScoreSlider.value);
        Debug.Log(purpleScoreSlider.value);
        Debug.Log(orangeScoreSlider.value);
	}


	// Initiates the creatures in the world according to the specified radius (number of creatures in each axis excluding the middle one)
	void initWorld (int gridRadius)
	{
		coordToCreature = new Creature[gridRadius + 1, gridRadius + 1, gridRadius + 1];

		// Create creatures and tiles:
		for (int x = 0; x <= gridRadius; ++x) {
			for (int y = 0; y <= gridRadius; ++y) {
				for (int z = 0; z <= gridRadius; ++z) {
					if (x == 0 || y == 0 || z == 0) {
						// Calculate world coordinates from cube-hexagon coordinates:
						float newX = 1.05f* (x - Mathf.Cos (Mathf.PI / 3) * (y + z));
						float newY = Random.value * 0.15f;
						float newZ = 0.8f * (Mathf.Sin (Mathf.PI / 3) * (y - z));

						// Create a new creature:
						Creature newCreature = Instantiate (CreaturePrefab)as Creature;
						newCreature.transform.parent = creatures.transform;
                        newCreature.transform.position = new Vector3(newX, newY, newZ);
                        Debug.Log(newZ);
                        newCreature.SetLayerOrders((int)Mathf.Abs(20 - 2*Mathf.Abs(5+newZ) ));

						// add creature to data structures
						coordToCreature [x, y, z] = newCreature;
						int[] coord = new int[3] { x, y, z };
						creatureToCoord.Add (newCreature, coord);

						// Create a new tile:
						GameObject newTile = Instantiate (hexTilePrefab);
						newTile.transform.parent = tiles.transform;
						newTile.transform.position = new Vector3 (newX, newY, newZ);


						++numCreatures;
					}
				}
			}
		}
	}
}
