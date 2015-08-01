using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tileScript : MonoBehaviour {

    public Sprite[] tileImages;
    public SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        sprite.sprite = tileImages[Random.Range(0, tileImages.Length - 1)]; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
