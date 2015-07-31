using UnityEngine;
using System.Collections;

public class TrajectoryManager : MonoBehaviour {
	LineRenderer trajectory;

	// Use this for initialization
	void Start () {
		/*
		trajectory = gameObject.AddComponent<LineRenderer> ();
		trajectory.SetWidth (0.2f, 0.1f);
		trajectory.material = new Material (Shader.Find("Particles/Additive"));
		trajectory.SetColors (Color.cyan, Color.blue);*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void init() 
	{

	}

	//trajectory mapping
	private void calibrateTrajectoryLineRenderer() 
	{

		
	}

	public Vector3 PlotTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time) {
		return start + startVelocity*time + Physics.gravity*time*time*0.5f;
	}
	
	public void PlotTrajectory (Vector3 start, Vector3 startVelocity, float timestep, float maxTime) {
		/*print ("plotting trajectory");
		Vector3 prev = start;
		
		//update line length
		trajectory.SetVertexCount ((int) (maxTime / timestep) + 2);
		
		for (int i=1;;i++) {
			float t = timestep*i;
			if (t > maxTime) break;
			Vector3 pos = PlotTrajectoryAtTime (start, startVelocity, t);
			print (pos);
			if (Physics.Linecast (prev,pos)) break;
			Debug.DrawLine (prev,pos,Color.red);
			
			//draw line
			trajectory.SetPosition(i, pos);
			
			
			prev = pos;
		}*/
	}
}
