using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class NextBallsWidget : MonoBehaviour {
	public GameObject topBall, middleBall, bottomBall;
	GameObject[] balls;
	Vector3[] dest;
	float[] stepSize;
	float epslion = 0.1f;
	int numberOfBallsInQueue;


	// Use this for initialization
	void Start () {
		numberOfBallsInQueue = Manager.instance.nextShots.Length;
		balls = new GameObject[numberOfBallsInQueue];
		balls [0] = bottomBall;
		balls [1] = middleBall;
		balls [2] = topBall;

		stepSize = new float[3];
		dest = new Vector3[3];
		upDateColors ();
	}
	
	// Update is called once per frame
	void Update () {
		
		for (int i = 0; i < numberOfBallsInQueue; i++) {
			RectTransform rectTrans = balls[i].GetComponent<RectTransform> ();
			if(Vector3.Distance(rectTrans.position, dest[i]) > epslion) {
				rectTrans.Translate((dest[i] - rectTrans.position).normalized*stepSize[i]);
			}
		}
		
	}

	public void upDateColors()
	{
		for (int i = 0; i < numberOfBallsInQueue; i++) {
			int j = (i + Manager.instance.nextShotIndex) % numberOfBallsInQueue;
			balls[i].GetComponent<Image>().color = Manager.instance.nextShots[j].GetColor();
		}
	}

	public void dropNextBall() 
	{
		//sort positions
		GameObject tmp = balls[0];

		balls [0] = balls [1];
		balls [1] = balls [2];
		balls [2] = tmp;

		balls [2].GetComponent<RectTransform> ().Translate(new Vector3(0, 60, 0));

		animateYMovement (0, -20, 2f);
		animateYMovement (1, -20, 2f);

		balls [2].GetComponent<Image> ().color = Manager.instance.nextShots [(((Manager.instance.nextShotIndex - 1) % numberOfBallsInQueue) + numberOfBallsInQueue) %  numberOfBallsInQueue].GetColor();
		animateYMovement (2, -20, 2f);
	}

	private void animateYMovement(int index, int deltaY, float step){
		stepSize [index] = step;
		dest[index] = new Vector3 (balls [index].GetComponent<RectTransform> ().position.x, balls [index].GetComponent<RectTransform> ().position.y + deltaY, balls [index].GetComponent<RectTransform> ().position.z);
	}
}
