using UnityEngine;
using System.Collections;
using System;

public class Shot : MonoBehaviour
{
	private Rigidbody rigidBody;
	private SpriteRenderer spriteRenderer;

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
	public ShotParams shotParams;

	protected void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
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
		if (transform.position.magnitude > 200) {
			Destroy (gameObject);
		}

		//make shots drop faster
		rigidBody.AddForce (-Vector3.up * gravityAddition);
	}
}
