using UnityEngine;

public class Billboard : MonoBehaviour { 
    void Update() {
        //Vector3 newVector = new Vector3(0, 0, 0);
        //transform.LookAt(Camera.main.transform.position + newVector, Vector3.up);
        transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position, Vector3.up); 
    } 
}
