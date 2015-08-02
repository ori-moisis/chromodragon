using UnityEngine;
using System.Collections;

public class TrajectoryManager : MonoBehaviour {
	LineRenderer trajectory;
	public float startWidth, endWidth, redrawThreshhold;
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
		hideTrajectory();
	}


	//trajectory mapping
	public void PlotTrajectory (Vector3 start, Vector3 velocity, Shot.ShotParams shotParams) {
		if (Vector3.Distance (velocity, lastVelocity) > redrawThreshhold) {
			lastVelocity = velocity;

			trajectory.SetPosition(0, start);
			trajectory.SetPosition(1, start + (velocity * directionIndecatorLengthMultiplier));

			if(velocity.y > 0) {
				trajectory.SetColors (shotParams.GetColor(), shotParams.GetColor());
			} else {
				hideTrajectory();
			}
		}
	}


	
	public void hideTrajectory() 
	{
		trajectory.SetColors (Color.clear, Color.clear);
	}

}
