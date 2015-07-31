using UnityEngine;
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

	int numGreen = 0;
	int numPurple = 0;
	int numOrange = 0;

	int numCreatures = 0;

	public int hexRadius = 3;
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


	void updateScore (GameColors prevColor, GameColors newColor)
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

		greenScoreSlider.value = numGreen / numCreatures;
		purpleScoreSlider.value = numPurple / numCreatures;
		orangeScoreSlider.value = numOrange / numCreatures;
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
						float newX = x - Mathf.Cos (Mathf.PI / 3) * (y + z);
						float newY = Random.value * 0.1f;
						float newZ = Mathf.Sin (Mathf.PI / 3) * (y - z);

						// Create a new creature:
						Creature newCreature = Instantiate (CreaturePrefab)as Creature;
						newCreature.transform.parent = creatures.transform;
						newCreature.transform.position = new Vector3 (newX, 0.25f, newZ);

						// add creature to data structures
						coordToCreature [x, y, z] = newCreature;
						int[] coord = new int[3] { x, y, z };
						creatureToCoord.Add (newCreature, coord);

						// Create a new tile:
						GameObject newTile = Instantiate (hexTilePrefab);
						newTile.transform.parent = tiles.transform;
						newTile.transform.position = new Vector3 (newX, newY + 0, newZ);


						++numCreatures;
					}
				}
			}
		}
	}
}
