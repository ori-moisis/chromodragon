using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class NextBallsWidget : MonoBehaviour {
	public GameObject topBall, middleBall, bottomBall;
	GameObject[] balls;
	float epslion = 0.001f;
	int numberOfBallsInQueue;
    public Vector2[] minDests;


	// Use this for initialization
	void Start () {
		numberOfBallsInQueue = Manager.instance.nextShots.Length;
		balls = new GameObject[numberOfBallsInQueue];
		balls [0] = bottomBall;
		balls [1] = middleBall;
		balls [2] = topBall;

        minDests = new Vector2[balls.Length];
        for (int i = 0; i < balls.Length; ++i )
        {
            RectTransform rectTrans = balls[i].GetComponent<RectTransform> ();
            minDests[i] = new Vector2(rectTrans.anchorMin.x, rectTrans.anchorMin.y);
        }

		upDateColors ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < numberOfBallsInQueue; i++) {
			RectTransform rectTrans = balls[i].GetComponent<RectTransform> ();
            if (Vector2.Distance(rectTrans.anchorMin, minDests[i]) > epslion)
            {
                rectTrans.anchorMin -= new Vector2(0f, 0.1f);
                rectTrans.anchorMax -= new Vector2(0f, 0.1f);
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

        balls[2].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        balls[2].GetComponent<RectTransform>().anchorMax = new Vector2(1, 1.4f);

        SetBallVisuals(2, Manager.instance.nextShots[(((Manager.instance.nextShotIndex - 1) % numberOfBallsInQueue) + numberOfBallsInQueue) % numberOfBallsInQueue]);
	}

    private void SetBallVisuals(int index, Shot.ShotParams param)
    {
        balls[2].GetComponent<Image>().color = param.GetColor();
        
    }
}
