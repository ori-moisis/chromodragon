using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
	public GameColors currentColor;
	private GameColors nextColor;

	public int amountToSpit;

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
		nextColor = currentColor;
		currentColor = GameColors.White;
		SetColor ();
	}

	public void EatColor (Shot shot)
	{
		if (currentColor.IsRivalColor (shot.shotParams.color)) {
			if (shot.shotParams.timeToLive > 0) {
				--shot.shotParams.timeToLive;
				SpitColor (shot);
			}
		} else {
			nextColor = currentColor.Add (shot.shotParams.color);
			Manager.instance.updateScore (currentColor, nextColor);
			animator.SetTrigger (EAT_TRIGGER);
		}
        
	}

	public void SpitColor (Shot shot)
	{
		var neighbors = Manager.instance.getNeighbours (this);

		int amountToDelete = neighbors.Count - amountToSpit;

		while (amountToDelete > 0) {
			int lastNeighbor = amountToDelete + amountToSpit - 1;
			int neighborToDelete = Random.Range (0, lastNeighbor);
			neighbors [neighborToDelete] = neighbors [lastNeighbor];
			neighbors [lastNeighbor] = null;
			--amountToDelete;
		}

		for (int i = 0; i < amountToSpit; ++i) {
			neighbors [i].EatColor (shot);
		}
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
	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "shot") {
			hit (col.gameObject.GetComponent<Shot> ());
			Destroy (col.gameObject);
		}
	}

	void hit (Shot shot)
	{
		switch (shot.shotParams.type) {
		case Shot.ShotTypes.ColorShot:
			EatColor (shot);
			break;
		//case Shot.ShotTypes.SpecialShot:
		// TODO: Special shot
		//Debug.Log ("Special shot");
		//break;
		}
	}
}
