using UnityEngine;
using System.Collections;

public class SlingshotGenerator : MonoBehaviour {
    public GameObject sligshot;
    public string slingshotName;
    public Vector3 slingshotPos;

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.inRoom)
        {
            Vector3 pos = Quaternion.Euler(0, 120 * (PhotonNetwork.player.ID - 1), 0) * slingshotPos;
            Quaternion rotation = Quaternion.LookRotation(-pos, Vector3.up);
            GameObject sling = (GameObject)PhotonNetwork.Instantiate(slingshotName, pos, rotation, 0);
            sling.GetComponentInChildren<Slingshot>().slingId = PhotonNetwork.player.ID;
        }
        else
        {
            int curId = 1;
            for (int i = 0; i < 3; ++i)
            {
                Vector3 pos = Quaternion.Euler(0, 120 * i, 0) * slingshotPos;
                GameObject sling = (GameObject)Instantiate(sligshot, pos, Quaternion.LookRotation(-pos, Vector3.up));
                sling.GetComponentInChildren<Slingshot>().slingId = curId;
                ++curId;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
