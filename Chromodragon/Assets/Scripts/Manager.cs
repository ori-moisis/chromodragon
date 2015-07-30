using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    public GameObject CreaturePrefab;

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
                        GameObject newCreature = Instantiate(CreaturePrefab);
                        float newX = x - Mathf.Cos(Mathf.PI/3)*( y + z);
                        float newZ = Mathf.Sin(Mathf.PI / 3) * (y - z);
                        newCreature.transform.position = new Vector3(newX, newZ, 0);
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
