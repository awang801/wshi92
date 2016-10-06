using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerLight : Tower
{

	int attackCharges;

	ParticleSystem particle;

	bool isAttacking;
	bool isCharging;
	float attackTime;
	float chargeTime;

	AudioClip shootingSFX;

	protected override void Awake()
	{
		base.Awake ();
		shootingSFX = (AudioClip)(Resources.Load("Sounds/LightChargeSFX", typeof(AudioClip)));
		shootSFX = (AudioClip)(Resources.Load("Sounds/LightFireSFX", typeof(AudioClip)));

		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Light", 3f, 3f, 40, 25, 60);

		chargeTime = 1f;

		particle = shootParticle.GetComponent<ParticleSystem> ();
		particle.Stop ();
	}

	protected override void Attack()
	{

		if (isAttacking == false) {
			sourceSFX.PlayOneShot(shootingSFX);
			attackCharges = 4;
			isCharging = true;
			isAttacking = true;
			StartCoroutine ("FireLight");

		} 

	}


	IEnumerator FireLight()
	{
		attackTime = 0f;
		particle.Play ();

		while (isAttacking) {

			attackTime += Time.fixedDeltaTime;
			if (isCharging) {
				
				if (attackTime >= chargeTime) isCharging = false;
		

			} else {
				
				if (attackTime >= 1f) {

					attackTime = 0;

					if (attackCharges <= 0 || sourceSFX.isPlaying == false) {

						attackCharges = 0;
						particle.Stop ();
						isAttacking = false;
						sourceSFX.Stop ();
						timeUntilAttack = attackDelay;

					} else if (!targetIsDead ()) {

						LightBullet myBullet = ((GameObject)(Instantiate (bullet, currentTargetT.position, bullet.transform.rotation))).GetComponent<LightBullet>();
						sourceSFX.PlayOneShot (shootSFX);
						myBullet.Setup (attackDamage);
						attackCharges -= 1;

					}
				}

			}

			yield return null;

		}

	}



}
