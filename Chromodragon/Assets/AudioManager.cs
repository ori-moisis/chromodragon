using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioClip slingshotBegin;
	public AudioClip slingshotStretch;
	public AudioClip slingshotShoot;
	private enum SlingshotState
	{
		Resting,
		Pulling,
		Shooting
	}
	private SlingshotState slingshotState;

	public AudioClip chomp;
	public AudioClip bleh;

	private AudioSource source;
	private static Dictionary<string, AudioClip> clips;


	protected void Awake ()
	{
		if (instance == null) {
			instance = this;
		}

		clips = new Dictionary<string, AudioClip> ();
		source = GetComponent<AudioSource> ();

		clips ["SlingshotBegin"] = slingshotBegin;
		clips ["SlingShotStretch"] = slingshotStretch;
		clips ["SlingshotShoot"] = slingshotShoot;

		clips ["Chomp"] = chomp;
		clips ["Bleh"] = bleh;
	}

	public static void PlayAudio (string name)
	{
		instance.source.PlayOneShot (clips [name]);
	}



	public static void StartSlingshot ()
	{
		if (instance.slingshotState == SlingshotState.Resting) {
			instance.StartCoroutine (instance.PlaySlingshot ());
		}
	}

	private IEnumerator PlaySlingshot ()
	{
		if (slingshotState != SlingshotState.Resting)
			yield break;

		slingshotState = SlingshotState.Pulling;

		Debug.Log ("Playing slingshotBegin");
		source.clip = slingshotBegin;
		source.Play ();
		yield return new WaitForSeconds (slingshotBegin.length);

		if (slingshotState != SlingshotState.Pulling)
			yield break;

		Debug.Log ("Playing slingshotStretch");
		source.clip = slingshotStretch;
		source.loop = true;
		source.Play ();
	}

	public static void EndSlingshot ()
	{
		instance.StartCoroutine (instance.StopSlingshot ());
	}

	private IEnumerator StopSlingshot ()
	{
		if (slingshotState != SlingshotState.Pulling) 
			yield break;
		;

		slingshotState = SlingshotState.Shooting;

		Debug.Log ("Playing slingshotShoot");
		source.clip = slingshotShoot;
		source.loop = false;
		source.Play ();
		yield return new WaitForSeconds (slingshotShoot.length);

		slingshotState = SlingshotState.Resting;		
	}
}
