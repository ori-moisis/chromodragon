using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

    public bool hasRed = false;
    public bool hasYellow = false;
    public bool hasBlue = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//collision callback
	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.tag == "shot")
		{
			// TODO: get color from col
			Destroy(col.gameObject);
			hit ();
		}
	}

	void hit()
	{
		//TODO: paint correctly
		Destroy(gameObject);
	}
}
