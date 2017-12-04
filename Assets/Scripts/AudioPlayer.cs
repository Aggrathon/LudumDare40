using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

	static AudioPlayer instance;

	[Header("Sources")]
	public AudioSource fx;
	[Header("Clips")]
	public AudioClip trumpet;
	public AudioClip fight;
	public AudioClip pling;

	private void Awake()
	{
		instance = this;
	}

	public static void PlayTrumpet()
	{
		instance.fx.PlayOneShot(instance.trumpet);
	}

	public static void PlayFight()
	{
		instance.fx.PlayOneShot(instance.fight);
	}

	public static void PlayPling()
	{
		instance.fx.PlayOneShot(instance.pling);
	}
}
