using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    public GameObject CreaturePrefab;
    public GameObject hexTilePrefab;

    // Initiates the creatures in the world according to the specified radius (number of creatures in each axis excluding the middle one)
    void initWorld(int gridRadius)
    {
        Creature[, ,] hexGrid = new Creature[gridRadius, gridRadius, gridRadius];
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
                        newCreature.transform.position = new Vector3(newX, 0.25f, newZ);

                        // Create a new tile:
                        GameObject newTile = Instantiate(hexTilePrefab);
                        newTile.transform.position = new Vector3(newX, 0.01f, newZ);
                    }
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
	    initWorld(3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
