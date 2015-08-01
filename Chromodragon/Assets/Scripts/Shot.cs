using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Shot : MonoBehaviour
{
	private Rigidbody rigidBody;
	private SphereCollider collider;
	private SpriteRenderer spriteRenderer;
	private Vector3 startingPosition;

	public float yVelocity;

    // REMEMBER TO FILL shotTypeWeights FOR EVERY NEW TYPE
	public enum ShotTypes : int
	{
		ColorShot,
        WhiteShot
	}


    static Dictionary<ShotTypes, float> shotTypeWeights = new Dictionary<ShotTypes, float>(); // REMEMBER TO FILL THIS FOR EVERY NEW TYPE
	static GameColors[] possibleColors = new GameColors[] { GameColors.Red, GameColors.Yellow, GameColors.Blue };

	[Serializable]
	public class ShotParams
	{
		public ShotTypes type;
		public GameColors color;
		public int timeToLive;

		public ShotParams ()
		{
            float sumW = 0;
            foreach (var item in shotTypeWeights)
            {
                type = item.Key;
                sumW += item.Value;
            }
            int typeIndex = (int)(UnityEngine.Random.value * sumW);
            float currW = 0;
            foreach (var item in shotTypeWeights)
            {
                currW += item.Value;
                if (typeIndex < currW)
                {
                    type = item.Key;
                    break;
                }
            }
            
			color = possibleColors [(int)(UnityEngine.Random.value * possibleColors.Length)];
			timeToLive = 2;
		}

		public ShotParams (ShotTypes type, GameColors color, int timeToLive)
		{
			this.type = type;
			this.color = color;
			this.timeToLive = timeToLive;
		}

        public Color GetColor()
        {
            switch (this.type)
            {
                case ShotTypes.ColorShot:
                    return ColorsManager.colorMap[this.color];
                case ShotTypes.WhiteShot:
                    return GameColors.White.GetColor();
                default:
                    return GameColors.White.GetColor();
            }
        }   
	}

	public float gravityAddition;
	public float velocityMultiplier;
	public ShotParams shotParams;
	public int framesToSelfDestruct = 60;

	protected void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
		collider = GetComponent<SphereCollider> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		startingPosition = transform.position;

        shotTypeWeights[ShotTypes.ColorShot] = 10;
        shotTypeWeights[ShotTypes.WhiteShot] = 1;
	}

	public void InitShot (ShotParams shotParams)
	{
		this.shotParams = shotParams;
        this.SetColor(this.shotParams.GetColor());
	}

	private void SetColor (Color color)
	{
		spriteRenderer.color = color;
	}

	void FixedUpdate ()
	{
		//make shots dsapear if their off screen for too long
		if (transform.position.magnitude > 150) {
			Destroy (gameObject);
		}

		float distFromStart = Vector3.Distance (startingPosition, transform.position);

		if (distFromStart > 7) {
			spriteRenderer.sortingOrder = -1;
		}

		//make shots drop faster
		if (rigidBody != null) {
//			Debug.Log ("Velocity: " + rigidBody.velocity.y);
			yVelocity = rigidBody.velocity.y;
			rigidBody.AddForce (-Vector3.up * gravityAddition);
			if (collider.enabled == false && rigidBody.velocity.y < 0) {
				collider.enabled = true;
				Debug.Log ("Enabling collider");
			}
		} else {
			//start self destruct timer
			if (framesToSelfDestruct < 0) {
				Destroy (gameObject);
			}
			framesToSelfDestruct--;
		}
	}

	public void hit (GameObject creature)
	{
		spriteRenderer.enabled = false;
		Destroy (rigidBody);
		//Destroy (gameObject);
	}
}
