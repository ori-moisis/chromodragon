using UnityEngine;
using System.Collections;

public class SlingshotGenerator : MonoBehaviour {
    public GameObject sligshot;
    public Vector3[] positions;

	// Use this for initialization
	void Start () {
        int curId = 1;
        foreach (Vector3 pos in positions)
        {
            GameObject sling = (GameObject)Instantiate(sligshot, pos, Quaternion.LookRotation(-pos, new Vector3(0,1,0)));
            sling.GetComponent<Slingshot>().slingId = curId;
            ++curId;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
