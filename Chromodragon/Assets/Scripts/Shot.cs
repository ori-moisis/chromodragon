using UnityEngine;
using System.Collections;
using System;

public class Shot : MonoBehaviour
{
	private Rigidbody rigidBody;
	private SpriteRenderer spriteRenderer;
	private Vector3 startingPosition;

	public enum ShotTypes : int
	{
		ColorShot
		//SpecialShot //?
	}

	static GameColors[] possibleColors = new GameColors[] { GameColors.Red, GameColors.Yellow, GameColors.Blue };
	static Array possibleTypes = Enum.GetValues (typeof(ShotTypes));

	[Serializable]
	public class ShotParams
	{
		public ShotTypes type;
		public GameColors color;
		public int timeToLive;

		public ShotParams ()
		{
			type = (ShotTypes)possibleTypes.GetValue ((int)(UnityEngine.Random.value * possibleTypes.Length));
			color = possibleColors [(int)(UnityEngine.Random.value * possibleColors.Length)];
			timeToLive = 2;
		}

		public ShotParams (ShotTypes type, GameColors color)
		{
			this.type = type;
			this.color = color;
			timeToLive = 2;
		}
	}

	public float gravityAddition;
	public float velocityMultiplier;
	public ShotParams shotParams;
	public int framesToSelfDestruct = 60;

	protected void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		startingPosition = transform.position;
	}

	public void InitShot (ShotParams shotParams)
	{
		this.shotParams = shotParams;
		this.SetColor (ColorsManager.colorMap [this.shotParams.color]);
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
			rigidBody.AddForce (-Vector3.up * gravityAddition);
		} else {
			//start self destruct timer
			if(framesToSelfDestruct < 0){
				Destroy(gameObject);
			}
			framesToSelfDestruct--;
		}
	}

	public void hit(GameObject creature)
	{
		spriteRenderer.enabled = false;
		Destroy (rigidBody);
		//Destroy (gameObject);
	}
}
