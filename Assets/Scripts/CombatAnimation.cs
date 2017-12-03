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
	}

	public void PlayBlock()
	{
		anim.SetTrigger(block);
	}

	public void PlayAttack()
	{
		anim.SetTrigger(attack);
	}

	public void PlayDamage()
	{
		blood.Play();
	}

	public void SetLook(Sprite s)
	{
		sprite.sprite = s;
	}
}
