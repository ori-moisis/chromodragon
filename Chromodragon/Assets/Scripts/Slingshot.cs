using UnityEngine;
using System.Collections;

public class Slingshot : Photon.PunBehaviour
{
	Vector3 screenPoint, offset, initialPosition;
	public Vector3 mozzleOffset; //where shot will apear compared to this
	public float velocityMultiplier; //how strong to shot compared to pull
	public GameObject shot; //what to shoot
	public bool debugPrints = false;
	public int slingId;
	public GameObject ballToShootHint;
	SpriteRenderer ballToSoohtHintRenderer;
	PhotonView photonView;
	LineRenderer rubberBand;
	TrajectoryManager trajectoryMngr;
	Vector3 orthogonalSideOffset;

	// Use this for initialization
	void Start ()
	{
		photonView = GetComponent<PhotonView> ();
		calibrateRubberBand ();
		trajectoryMngr = GetComponentInChildren<TrajectoryManager> ();
		if (PhotonNetwork.inRoom && ! photonView.isMine) {
			this.transform.parent.GetComponent<Renderer> ().enabled = false;
			GetComponentInParent<Renderer> ().enabled = false;
		}
		ballToSoohtHintRenderer = ballToShootHint.GetComponent<SpriteRenderer> ();
		ballToSoohtHintRenderer.color = Color.clear;

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	bool IsDisabled ()
	{
		if (! PhotonNetwork.inRoom) {
			return false;
		}
		if (! photonView.isMine) {
			return true;
		}
		if (! Manager.instance.isMyTurn ()) {
			return true;
		}
		return false;
	}

	//start to drag
	void OnMouseDown ()
	{
		if (this.IsDisabled ()) {
			return;
		}
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		initialPosition = gameObject.transform.position;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		if (debugPrints)
			print ("offset - " + offset);



	}
	
	void OnMouseDrag ()
	{
		if (this.IsDisabled ()) {
			return;
		}

		//calculate new dragged position
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
		transform.position = curPosition;
		updateRubberBand ();
		
		//plot trajectory
		Vector3 diff = initialPosition - transform.position;
		Shot.ShotParams sParams = Manager.instance.nextShots [Manager.instance.nextShotIndex];
		trajectoryMngr.PlotTrajectory (initialPosition, diff * velocityMultiplier, sParams);
		ballToSoohtHintRenderer.color = sParams.GetColor ();;

		AudioManager.StartSlingshot ();

		if (debugPrints)
			print ("curScreenPoint - " + curScreenPoint + "\ncurPosition - " + curPosition);
	}
	
	void OnMouseUp ()
	{
		ballToSoohtHintRenderer.color = Color.clear;

		if (this.IsDisabled ()) {
			return;
		}

		//calculate drag vector
		Vector3 diff = initialPosition - transform.position;

		if (debugPrints)
			print ("initial = " + initialPosition + "\ncurrent position = " + transform.position + "\nvec = " + diff + "\ndist = " + diff.magnitude);

		//snap draggable back
		transform.position = initialPosition;
		updateRubberBand ();

		//hide guide
		trajectoryMngr.hideTrajectory ();

		//shoot
		AudioManager.EndSlingshot ();

		if (diff.y > 0) {
			Shot.ShotParams nextShot = Manager.instance.GetNextShot ();
			if (PhotonNetwork.inRoom) {
				PhotonNetwork.RPC (photonView, "shoot", PhotonTargets.All, false, new object[] {
					diff * velocityMultiplier,
					(int)nextShot.type,
					(int)nextShot.color,
					nextShot.timeToLive
				});
			} else {
				shoot (diff * velocityMultiplier, (int)nextShot.type, (int)nextShot.color, nextShot.timeToLive);
			}
		}

	}

    

	//shoot
	[PunRPC]
	void shoot (Vector3 dir, int intType, int intColor, int timeToLive)
	{
		if (debugPrints)
			print ("Shooot!");
		Shot.ShotTypes type = (Shot.ShotTypes)intType;
		GameColors color = (GameColors)intColor;
		GameObject myShot = (GameObject)Instantiate (shot);
		myShot.GetComponent<Shot> ().InitShot (new Shot.ShotParams (type, color, timeToLive));
		var shotRigidBody = myShot.GetComponent<Rigidbody> ();
		shotRigidBody.transform.position = this.transform.position + mozzleOffset;
		shotRigidBody.velocity = dir;
		Manager.instance.updateTurn ();
	}

	//calibrate rubber bands once
	void calibrateRubberBand ()
	{
		rubberBand = GetComponentInParent<LineRenderer> ();
		orthogonalSideOffset = Vector3.Cross (-transform.position, Vector3.up);
		orthogonalSideOffset.Normalize ();
		orthogonalSideOffset.Scale (new Vector3 (0.4f, 0.4f, 0.4f));
		rubberBand.SetPosition (0, transform.position + orthogonalSideOffset);
		rubberBand.SetPosition (1, transform.position + orthogonalSideOffset/2);
		rubberBand.SetPosition (2, transform.position - orthogonalSideOffset/2);
		rubberBand.SetPosition (3, transform.position - orthogonalSideOffset);
		updateRubberBand ();
	}

	//update rubber band location
	void updateRubberBand ()
	{
		rubberBand.SetPosition (1, transform.position + orthogonalSideOffset/2);
		rubberBand.SetPosition (2, transform.position - orthogonalSideOffset/2);
	}





}

