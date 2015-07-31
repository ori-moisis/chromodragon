using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
	Rigidbody rb;
	public float gravityAddition;

	void Start() {
		rb = GetComponent<Rigidbody> ();

	}

	void FixedUpdate() {
		//make shots dsapear if their off screen for too long
		if (transform.position.magnitude > 200) {
			print ("i want to die");
			Destroy(gameObject);
		}

		//make shots drop faster
		rb.AddForce (-Vector3.up * gravityAddition);
	}
}