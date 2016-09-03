using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerCannon : Tower
{

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/CannonFireSFX", typeof(AudioClip)));
		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Cannon", 2f, 3f, 40, 25, 60);
	}

	protected override void Attack()
	{
		Bullet newBullet = ((GameObject)(Instantiate(bullet, bulletPointTransform.position, Quaternion.identity))).GetComponent<Bullet>();
		Instantiate (shootParticle, bulletPointTransform.position, bulletPointTransform.rotation);
		newBullet.Setup(currentTargetUnit, attackDamage, 25f);

		timeUntilAttack = attackDelay;

		sourceSFX.PlayOneShot(shootSFX);
	}

}
