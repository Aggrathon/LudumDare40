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
	public AudioClip ching;
	public AudioClip huh;
	public AudioClip hoah;

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

	public static void PlayChing()
	{
		instance.fx.PlayOneShot(instance.ching);
	}

	public static void PlayHuh()
	{
		instance.fx.PlayOneShot(instance.huh);
	}

	public static void PlayHoah()
	{
		instance.fx.PlayOneShot(instance.hoah);
	}
}
