using UnityEngine;
using System.Collections;

public class Slingshot : MonoBehaviour {
	Vector3 screenPoint, offset, initialPosition;
	public Vector3 mozzleOffset; //where shot will apear compared to this
	public float velocityMultiplier; //how strong to shot compared to pull
	public GameObject shot; //what to shoot
	public bool debugPrints = false;
    public int slingId;
    PhotonView photonView;
	LineRenderer rubberBand;
	TrajectoryManager trajectoryMngr;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
		calibrateRubberBand ();
		trajectoryMngr = GetComponentInChildren<TrajectoryManager> ();
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
		updateRubberBand ();
		
		//plot trajectory
		Vector3 diff = initialPosition - transform.position;
		trajectoryMngr.PlotTrajectory (initialPosition,  diff * velocityMultiplier );

		if(debugPrints) print ("curScreenPoint - " + curScreenPoint + "\ncurPosition - " + curPosition);
	}
	
	void OnMouseUp()
	{
		//calculate drag vector
		Vector3 diff = initialPosition - transform.position;

		if(debugPrints) print("initial = " + initialPosition + "\ncurrent position = " + transform.position + "\nvec = " + diff + "\ndist = " + diff.magnitude);

		//snap draggable back
		transform.position = initialPosition;
		updateRubberBand ();

		//hide guide
		trajectoryMngr.hideTrajectory ();

		//shoot
		if (diff.y > 0) {
            if (PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.player.ID == slingId)
                {
                    PhotonNetwork.RPC(photonView, "shoot", PhotonTargets.All, false, new object[] {this.transform.position, diff * velocityMultiplier});
                }
            }
            else
            {
                shoot(this.transform.position, diff * velocityMultiplier);
            }
		}
	}

	//shoot
    [PunRPC]
	void shoot(Vector3 pos, Vector3 dir)
	{
		print("shooting - velocity is - " + dir);
		var myShot = Instantiate (shot);
		var shotRigidBody = myShot.GetComponent<Rigidbody> ();
        shotRigidBody.transform.position = this.transform.position + mozzleOffset;
		shotRigidBody.velocity = dir;		
	}

	//calibrate rubber bands once
	void calibrateRubberBand()
	{
		rubberBand = GetComponentInParent<LineRenderer> ();
		var orthogonalSideOffset = Vector3.Cross (-transform.position, Vector3.up);
		orthogonalSideOffset.Normalize ();
		rubberBand.SetPosition (0, transform.position + orthogonalSideOffset);
		rubberBand.SetPosition (2, transform.position - orthogonalSideOffset);
		updateRubberBand();
	}

	//update rubber band location
	void updateRubberBand(){
		rubberBand.SetPosition (1, transform.position);
	}





}

