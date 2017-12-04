using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatAnimation : MonoBehaviour {

	public ParticleSystem cheer;
	public ParticleSystem blood;
	public SpriteRenderer sprite;

	Animator anim;
	int utility = Animator.StringToHash("Utility");
	int block = Animator.StringToHash("Block");
	int attack = Animator.StringToHash("Attack");

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	[ContextMenu("Play Utility")]
	public void PlayUtility()
	{
		cheer.Play();
		anim.SetTrigger(utility);
		StartCoroutine(DelaydHoah());
	}

	public void PlayBlock()
	{
		anim.SetTrigger(block);
		StartCoroutine(DelaydHuh());
	}

	public void PlayAttack()
	{
		anim.SetTrigger(attack);
		StartCoroutine(DelaydChing());
	}

	public void PlayDamage()
	{
		blood.Play();
	}

	public void SetLook(Sprite s)
	{
		sprite.sprite = s;
	}

	IEnumerator DelaydChing()
	{
		yield return new WaitForSeconds(0.25f);
		AudioPlayer.PlayChing();
	}

	IEnumerator DelaydHoah()
	{
		yield return new WaitForSeconds(0.1f);
		AudioPlayer.PlayHoah();
	}
	IEnumerator DelaydHuh()
	{
		yield return new WaitForSeconds(0.05f);
		AudioPlayer.PlayHuh();
	}
}
