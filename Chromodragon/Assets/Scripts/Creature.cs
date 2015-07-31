using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{

	public GameColors currentColor;
	private GameColors nextColor;

	private SpriteRenderer sprite;
	private Animator animator;

	private static int EAT_TRIGGER = Animator.StringToHash ("eat");

#if UNITY_EDITOR
	protected void OnDrawGizmos ()
	{
		Awake ();
	}
#endif

	protected void Awake ()
	{
		sprite = GetComponentInChildren<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
	}

	protected void Update ()
	{
	
	}

	public void EatColor (GameColors color)
	{
		nextColor = currentColor.Add (color);
		animator.SetTrigger (EAT_TRIGGER);
	}

	public void ClearColors ()
	{
		if (currentColor != GameColors.White) {
			nextColor = GameColors.White;
			SetColor ();
		}
	}

	private void SetColor ()
	{
		if (nextColor != currentColor) {
			currentColor = nextColor;
			sprite.color = currentColor.GetColor ();
		}
	}

	//collision callback
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "shot") {
//			Destroy (col.gameObject);
			col.gameObject.SetActive (false);
			hit (col.gameObject.GetComponent<Shot> ());
		}
	}

	void hit (Shot shot)
	{
		switch (shot.shotType) {
		case Shot.ShotTypes.ColorShot:
			EatColor (shot.shotColor);
			break;
		case Shot.ShotTypes.SpecialShot:
			// TODO: Special shot
			Debug.Log ("Special shot");
			break;
		}
	}
}
