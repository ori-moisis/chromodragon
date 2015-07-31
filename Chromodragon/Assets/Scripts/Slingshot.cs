using UnityEngine;
using System.Collections;

public class Slingshot : Photon.PunBehaviour {
	Vector3 screenPoint, offset, initialPosition;
	public Vector3 mozzleOffset; //where shot will apear compared to this
	public float velocityMultiplier; //how strong to shot compared to pull
	public GameObject shot; //what to shoot
	public bool debugPrints = false;
    public int slingId;
    PhotonView photonView;
	LineRenderer rubberBand;
	TrajectoryManager trajectoryMngr;
    Shot.ShotParams[] nextShots;
    int nextShotIndex;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
		calibrateRubberBand ();
		trajectoryMngr = GetComponentInChildren<TrajectoryManager> ();
		trajectoryMngr.init ();
        nextShots = new Shot.ShotParams[3];
        nextShotIndex = 0;
        for (int i = 0; i < nextShots.Length; ++i)
        {
            nextShots[i] = new Shot.ShotParams();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool IsDisabled()
    {
        if (! PhotonNetwork.inRoom)
        {
            return false;
        }
        if (! photonView.isMine)
        {
            return true;
        }
        if ((Manager.instance.currentTurn + 1) != PhotonNetwork.player.ID)
        {
            return true;
        }
        return false;
    }

	//start to drag
	void OnMouseDown() {
        if (this.IsDisabled())
        {
            return;
        }
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		initialPosition = gameObject.transform.position;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		if(debugPrints) print("offset - " + offset);
	}
	
	void OnMouseDrag() 
	{
        if (this.IsDisabled())
        {
            return;
        }
        //calculate new dragged position
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
		updateRubberBand ();
		
		//plot trajectory
		Vector3 diff = initialPosition - transform.position;
		trajectoryMngr.PlotTrajectory (this.transform.position + mozzleOffset, diff * velocityMultiplier, 0.5f, 5);

		if(debugPrints) print ("curScreenPoint - " + curScreenPoint + "\ncurPosition - " + curPosition);
	}
	
	void OnMouseUp()
	{
        if (this.IsDisabled())
        {
            return;
        }

		//calculate drag vector
		Vector3 diff = initialPosition - transform.position;

		if(debugPrints) print("initial = " + initialPosition + "\ncurrent position = " + transform.position + "\nvec = " + diff + "\ndist = " + diff.magnitude);

		//snap draggable back and shoot
		transform.position = initialPosition;
		updateRubberBand ();

		if (diff.y > 0) {
            Shot.ShotParams nextShot = this.GetNextShot();
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.RPC(photonView, "shoot", PhotonTargets.All, false, new object[] {diff * velocityMultiplier, (int)nextShot.type, (int)nextShot.color});
            }
            else
            {
                shoot(diff * velocityMultiplier, (int)nextShot.type, (int)nextShot.color);
            }
		}
	}

    Shot.ShotParams GetNextShot()
    {
        Shot.ShotParams next = this.nextShots[this.nextShotIndex];
        this.nextShots[this.nextShotIndex] = new Shot.ShotParams();
        this.nextShotIndex = (this.nextShotIndex + 1) % this.nextShots.Length;
        return next;
    }

	//shoot
    [PunRPC]
	void shoot(Vector3 dir, int intType, int intColor)
	{
		if(debugPrints) print("Shooot!");
        Shot.ShotTypes type = (Shot.ShotTypes)intType;
        GameColors color = (GameColors)intColor;
		GameObject myShot = (GameObject)Instantiate (shot);
        myShot.GetComponent<Shot>().InitShot(new Shot.ShotParams(type, color));
		var shotRigidBody = myShot.GetComponent<Rigidbody> ();
        shotRigidBody.transform.position = this.transform.position + mozzleOffset;
		shotRigidBody.velocity = dir;
        Manager.instance.currentTurn = (Manager.instance.currentTurn + 1) % PhotonNetwork.playerList.Length;
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

