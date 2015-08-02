using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Shot : MonoBehaviour
{
	private Rigidbody rigidBody;
	private SphereCollider shotCollider;
	private SpriteRenderer spriteRenderer;
	private Vector3 startingPosition;
	public GameObject[] effects;

	public float yVelocity;

    // REMEMBER TO FILL shotTypeWeights FOR EVERY NEW TYPE
	public enum ShotTypes : int
	{
		ColorShot,
        WhiteShot,
        InstantBlech,
		BombShot
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
            if (type == ShotTypes.InstantBlech)
            {
                timeToLive = 1;
            }
            else 
            {
                timeToLive = 2;
            }

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
                    return this.color.GetColor();
                case ShotTypes.InstantBlech:
                    Color col = this.color.GetColor();
                    return Color.Lerp(col, Color.black, 0.4f); 
				case ShotTypes.BombShot:
					return Color.black;
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
		shotCollider = GetComponent<SphereCollider> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		startingPosition = transform.position;

        shotTypeWeights[ShotTypes.ColorShot] = 6;
        shotTypeWeights[ShotTypes.WhiteShot] = 1;
        shotTypeWeights[ShotTypes.InstantBlech] = 1;
		shotTypeWeights[ShotTypes.BombShot] = 1;
	}

	public void InitShot (ShotParams shotParams)
	{
		this.shotParams = shotParams;
        this.SetColor(this.shotParams.GetColor());


		//effects
		if(shotParams.type == ShotTypes.BombShot)
		{
			//spriteRenderer.enabled = false;
			GameObject effect = Instantiate(effects[0]);
			effect.transform.position = transform.position;
			effect.transform.parent = transform;
		}
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
			if (shotCollider.enabled == false && rigidBody.velocity.y < 0) {
				shotCollider.enabled = true;
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
		if (shotParams.type == ShotTypes.BombShot) {
			GameObject effect = Instantiate (effects [1]);
			effect.transform.position = transform.position;
			effect.transform.parent = transform; //this is to remove object eventually
		}
		spriteRenderer.enabled = false;
		Destroy (rigidBody);
		//Destroy (gameObject);
	}
}
