using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerMagic : Tower
{

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/MagicSFX", typeof(AudioClip)));
		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Magic", 3f, 1f, 20, 15, 35);
	}

	protected override void Attack()
	{
		Bullet newBullet = ((GameObject)(Instantiate(bullet, bulletPointTransform.position, bullet.transform.rotation))).GetComponent<Bullet>();
		Instantiate (shootParticle, bulletPointTransform.position, bulletPointTransform.rotation);
		newBullet.Setup(currentTargetUnit, attackDamage, 10f);

		timeUntilAttack = attackDelay;

		sourceSFX.PlayOneShot(shootSFX);
	}

}
