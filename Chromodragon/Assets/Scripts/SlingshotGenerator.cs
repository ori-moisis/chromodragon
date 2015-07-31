using UnityEngine;
using System.Collections;

public class SlingshotGenerator : MonoBehaviour {
    public GameObject sligshot;
    public string slingshotName;
    public Vector3[] positions;

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.inRoom)
        {
            Vector3 pos = positions[PhotonNetwork.player.ID - 1];
            Quaternion rotation = Quaternion.LookRotation(-pos, Vector3.up);
            GameObject sling = (GameObject)PhotonNetwork.Instantiate(slingshotName, pos, rotation, 0);
            sling.GetComponentInChildren<Slingshot>().slingId = PhotonNetwork.player.ID;
        }
        else
        {
            int curId = 1;
            foreach (Vector3 pos in positions)
            {
                GameObject sling = (GameObject)Instantiate(sligshot, pos, Quaternion.LookRotation(-pos, new Vector3(0, 1, 0)));
                sling.GetComponentInChildren<Slingshot>().slingId = curId;
                ++curId;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
