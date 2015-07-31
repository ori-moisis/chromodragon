using UnityEngine;
using System.Collections;

public class TrajectoryManager : MonoBehaviour {
	LineRenderer trajectory;
	public float startWidth, endWidth, /*timestep, maxTime, */ redrawThreshhold;
	public Material material;
	public Color startColor, endColor;
	public float directionIndecatorLengthMultiplier;

	Vector3 lastVelocity;



	// Use this for initialization
	void Start () {
		trajectory = gameObject.AddComponent<LineRenderer> ();
		trajectory.SetWidth (startWidth, endWidth);
		trajectory.material = material;
		trajectory.SetVertexCount (2);
		trajectory.useWorldSpace = true;
		lastVelocity = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	//trajectory mapping
	private void calibrateTrajectoryLineRenderer() 
	{

		
	}
	/*
	public Vector3 PlotTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time) {
		return start + startVelocity * time + Physics.gravity*(time*time)*0.5f;
	}	
	*/
	public void PlotTrajectory (Vector3 start, Vector3 startVelocity, Shot.ShotParams shotParams) {
		if (Vector3.Distance (startVelocity, lastVelocity) > redrawThreshhold) {
			lastVelocity = startVelocity;
			print ("plotting trajectory - Velocity is -" + startVelocity);
			Vector3 prev = start;


			/*
			//update line length
			trajectory.SetVertexCount(0);
			trajectory.setVertexCount((int)(time / timestep) + 2)

			//update line positions
			for (int i=1;; i++) {
				float t = timestep * i;
				if (t > maxTime)
					break;
				Vector3 pos = PlotTrajectoryAtTime (start, startVelocity, t);
				if (Physics.Linecast (prev, pos))
					break;
				Debug.DrawLine (prev, pos, Color.red);
				
				//draw line
				trajectory.SetPosition (i, pos);
				
				
				prev = pos;													
			}
			*/
			trajectory.SetPosition(0, start);
			trajectory.SetPosition(1, start + (startVelocity * directionIndecatorLengthMultiplier));


			trajectory.SetColors (ColorsManager.colorMap[shotParams.color], ColorsManager.colorMap[shotParams.color]);
		}
	}


	
	public void hideTrajectory() 
	{
		trajectory.SetColors (Color.clear, Color.clear);
	}

}
