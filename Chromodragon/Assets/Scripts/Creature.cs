using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
	public GameColors currentColor;
	public GameObject shotPrefab;
	public bool iShoot;

	private Shot.ShotParams savedShotParams;

	public float spitAngle;
	public int spitDirections;
	public float muzzleOffset;
	public float spitSpeed;
	private List<Vector3> spitDirectionVectors;

	private SpriteRenderer sprite;
	private Animator animator;

	private static int EAT_TRIGGER = Animator.StringToHash ("eat");

//#if UNITY_EDITOR
//	protected void OnDrawGizmos ()
//	{
//		Awake ();
//	}
//#endif

	protected void Awake ()
	{
		sprite = GetComponentInChildren<SpriteRenderer> ();
		animator = GetComponent<Animator> ();

		CalculateSpitDirections ();
	}

	private float time = 0;
	protected void Update ()
	{
		if (iShoot) {
			time += Time.deltaTime;
			if (time > 1) {
				time = 0;
				CalculateSpitDirections ();
				SpitShot (new Shot.ShotParams ());
			}
		}
	}

	private void CalculateSpitDirections ()
	{
		spitDirectionVectors = new List<Vector3> ();
		Vector3 direction = Quaternion.Euler (0, 0, -spitAngle) * new Vector3 (1, 0, 0);
		for (int i = 0; i < spitDirections; i++) {
			spitDirectionVectors.Add (direction);
			direction = Quaternion.Euler (0, 360 / spitDirections, 0) * direction;
		}
	}

	public void SpitShot (Shot.ShotParams shotParams)
	{
		foreach (var direction in spitDirectionVectors) {
//			Debug.Log (direction);
			ShootColor (direction, shotParams);
		}
	}

	private void ShootColor (Vector3 direction, Shot.ShotParams shotParams)
	{
//		float height = direction.y;
//		direction.y = 0;

//		float distance = direction2d.magnitude;
//		float angle = spitAngle * Mathf.Deg2Rad;
//		Vector3 direction3d = new Vector3 (direction2d.x, distance * Mathf.Tan (angle), direction2d.y);
//		distance += height / Mathf.Tan (angle);
//		float speed = Mathf.Sqrt (distance * Physics.gravity.magnitude / Mathf.Sin (2 * angle));
//		Vector3 direction3d = new Vector3 (direction2d.x, spitHeight, direction2d.y);
//		Vector3 velocity = spitSpeed * direction;

		Shot newShot = Object.Instantiate (shotPrefab).GetComponent<Shot> ();
		newShot.InitShot (new Shot.ShotParams (shotParams.type, shotParams.color, shotParams.timeToLive - 1));
		var shotRigidBody = newShot.GetComponent<Rigidbody> ();
		shotRigidBody.transform.position = this.transform.position + new Vector3 (0, muzzleOffset, 0);
		shotRigidBody.velocity = spitSpeed * direction;
	}

	//collision callback
	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "shot") {
			hit (col.gameObject.GetComponent<Shot> ());
			col.GetComponent<Shot> ().hit (gameObject);
		}
	}

	void hit (Shot shot)
	{
		EatColor (shot.shotParams);
	}

	public void EatColor (Shot.ShotParams shotParams)
	{
		savedShotParams = shotParams;
		animator.SetTrigger (EAT_TRIGGER);
	}
	
	private void ConsumeShot ()
	{
		Debug.Log ("POOOOOOOP");
		AudioManager.PlayAudio ("Gulp");

		switch (savedShotParams.type) {
		case Shot.ShotTypes.ColorShot:
			{
				GameColors newColor = currentColor.Add (savedShotParams.color);

				if (currentColor.IsRivalColor (savedShotParams.color)) {
					if (savedShotParams.timeToLive > 0) {
						SpitShot (savedShotParams);
					}
				} else if (newColor != currentColor) {
					Manager.instance.updateScore (currentColor, newColor);
					currentColor = newColor;
					sprite.color = currentColor.GetColor ();

				}
			}
			break;
		case Shot.ShotTypes.WhiteShot:
			{
				if (currentColor != GameColors.White) {
					Manager.instance.updateScore (currentColor, GameColors.White);
					currentColor = GameColors.White;
					sprite.color = currentColor.GetColor ();
				}
			}
			break;
		case Shot.ShotTypes.InstantBlech:
			{
				GameColors newColor = savedShotParams.color;
				Manager.instance.updateScore (currentColor, newColor);
				currentColor = newColor;
				sprite.color = currentColor.GetColor ();
				if (savedShotParams.timeToLive > 0) {
					SpitShot (savedShotParams);
				}
			}
			break;
		}
	}
}
