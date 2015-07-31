using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

    public static Manager instance;

    public GameObject CreaturePrefab;
    public GameObject hexTilePrefab;
    public Vector3[] cameraPositions;
    public GameObject creatures;
    public GameObject tiles;

    public int hexRadius = 3;

    // Map hexagon-cube coordinates to creatures:
    GameObject[, ,] coordToCreature;

    // Map creaturee to hexagon-cube coordinates:
    Dictionary<GameObject, int[]> creatureToCoord = new Dictionary<GameObject, int[]>();

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        initWorld(hexRadius);
	    if (PhotonNetwork.inRoom)
        {
            Camera.main.transform.position = cameraPositions[PhotonNetwork.player.ID - 1];
            Camera.main.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position, new Vector3(0, 1, 0));
        }
	}

	
	// Update is called once per frame
	void Update () {
	    
	}


    public GameObject[] getNeighbours(GameObject creature)
    {
        GameObject[] neighbours = new GameObject[6]{null, null, null, null, null, null};
        int[] coord = creatureToCoord[creature];

        if (coord[0] > 0)
        {
            neighbours[0] = coordToCreature[coord[0]-1, coord[1], coord[2]];
        }
        if (coord[0] < hexRadius)
        {
            neighbours[1] = coordToCreature[coord[0] + 1, coord[1], coord[2]];
        }

        if (coord[1] > 0)
        {
            neighbours[2] = coordToCreature[coord[0], coord[1] - 1, coord[2]];
        }
        if (coord[1] < hexRadius)
        {
            neighbours[3] = coordToCreature[coord[0], coord[1] + 1, coord[2]];
        }

        if (coord[2] > 0)
        {
            neighbours[4] = coordToCreature[coord[0], coord[1], coord[2] - 1];
        }
        if (coord[2] < hexRadius)
        {
            neighbours[5] = coordToCreature[coord[0], coord[1], coord[2] + 1];
        }

        return neighbours;
    }


    // Initiates the creatures in the world according to the specified radius (number of creatures in each axis excluding the middle one)
    void initWorld(int gridRadius)
    {
        coordToCreature = new GameObject[gridRadius + 1, gridRadius + 1, gridRadius + 1];

        // Create creatures and tiles:
        for (int x = 0; x <= gridRadius; ++x)
        {
            for (int y = 0; y <= gridRadius; ++y)
            {
                for (int z = 0; z <= gridRadius; ++z)
                {
                    if (x == 0 || y == 0 || z == 0)
                    {
                        // Calculate world coordinates from cube-hexagon coordinates:
                        float newX = x - Mathf.Cos(Mathf.PI / 3) * (y + z);
                        float newZ = Mathf.Sin(Mathf.PI / 3) * (y - z);

                        // Create a new creature:
                        GameObject newCreature = Instantiate(CreaturePrefab);
                        newCreature.transform.parent = creatures.transform;
                        newCreature.transform.position = new Vector3(newX, 0.25f, newZ);

                        // add creature to data structures
                        coordToCreature[x, y, z] = newCreature;
                        int[] coord = new int[3] { x, y, z };
                        creatureToCoord.Add(newCreature, coord);

                        // Create a new tile:
                        GameObject newTile = Instantiate(hexTilePrefab);
                        newTile.transform.parent = tiles.transform;
                        newTile.transform.position = new Vector3(newX, 0.01f, newZ);
                    }
                }
            }
        }
    }
}
