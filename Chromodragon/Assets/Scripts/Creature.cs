﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
	public GameColors currentColor;
	private GameColors nextColor;

	public int amountToSpit;

	public SpriteRenderer bodyRenderer;
    public SpriteRenderer eyesRenderer;


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
        animator = GetComponent<Animator>();
		nextColor = currentColor;
		currentColor = GameColors.White;
		SetColor ();
	}

    public void SetLayerOrders(int order)
    {
        //bodyRenderer.sortingOrder = 2*order;
        //eyesRenderer.sortingOrder = 2*order + 1;
    }

	public void EatColor (GameColors color)
	{
        GameColors prevColor = currentColor;
		if (currentColor.IsRivalColor (color)) {
//			SpitColor (color);
		} else {
			nextColor = currentColor.Add (color);
            Manager.instance.updateScore(prevColor, nextColor);
			animator.SetTrigger (EAT_TRIGGER);
		}
        
	}

	public void SpitColor (GameColors color)
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
//			if (neighbor != null) {
			Debug.Log ("Spitting to neighbor " + i);
			neighbors [i].EatColor (color);
//			}
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
			bodyRenderer.color = currentColor.GetColor ();
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
			EatColor (shot.shotParams.color);
			break;
		//case Shot.ShotTypes.SpecialShot:
			// TODO: Special shot
            //Debug.Log ("Special shot");
			//break;
		}
	}
}
