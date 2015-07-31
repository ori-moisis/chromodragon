using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{
	private Rigidbody rigidBody;
	private SpriteRenderer spriteRenderer;
	private TrailRenderer trailRenderer;

	public float gravityAddition;

	public enum ShotTypes
	{
		ColorShot,
		SpecialShot //?
	}

	public ShotTypes shotType;
	public GameColors shotColor;

#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if (!Application.isPlaying) {
			Awake ();
		}
	}
#endif

	protected void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		trailRenderer = GetComponent<TrailRenderer> ();

		// TODO: make this not random
		GameColors color;
		float randomColorPercent = Random.value * 3;
		if (randomColorPercent < 1.0f) {
			color = GameColors.Blue;
		} else if (randomColorPercent < 2.0f) {
			color = GameColors.Red;
		} else {
			color = GameColors.Yellow;
		}

		InitShot (ShotTypes.ColorShot, color);
	}

	public void InitShot (ShotTypes type, GameColors color)
	{
		shotType = type;
		shotColor = color;

		Debug.Log ("Setting shot color");

		switch (shotType) {
		case ShotTypes.ColorShot:
			spriteRenderer.color = ColorsManager.colorMap [shotColor];
			break;
		case ShotTypes.SpecialShot:
			spriteRenderer.color = Color.gray;
			break;
		}
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