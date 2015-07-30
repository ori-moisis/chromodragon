using UnityEngine;
using System.Collections;

public class ClickAndDragShooting : MonoBehaviour {
	Vector3 screenPoint, offset, initialPosition;
	public Vector3 mozzleOffset; //where shot will apear compared to this
	public float velocityMultiplier; //how strong to shot compared to pull
	public GameObject shot; //what to shoot
	public bool debugPrints = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//start to drag
	void OnMouseDown() {
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		initialPosition = gameObject.transform.position;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		if(debugPrints) print("offset - " + offset);
	}
	
	void OnMouseDrag() 
	{ 
		//calculate new dragged position
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;

		if(debugPrints) print ("curScreenPoint - " + curScreenPoint + "\ncurPosition - " + curPosition);
	}
	
	void OnMouseUp()
	{
		//calculate drag vector
		Vector3 diff = initialPosition - transform.position;

		if(debugPrints) print("initial = " + initialPosition + "\ncurrent position = " + transform.position + "\nvec = " + diff + "\ndist = " + diff.magnitude);

		//snap draggable back and shoot
		transform.position = initialPosition;
		shoot (diff * velocityMultiplier);
	}

	//shoot
	void shoot(Vector3 dir)
	{
		if(debugPrints) print("Shooot!");
		var myShot = Instantiate (shot);
		var shotRigidBody = myShot.GetComponent<Rigidbody> ();
		shotRigidBody.transform.position = this.transform.position + mozzleOffset;
		shotRigidBody.velocity = dir;
	}
}

