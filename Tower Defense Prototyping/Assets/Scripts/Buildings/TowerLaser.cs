using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerLaser : Tower
{

	LineRenderer laser;
	bool isAttacking;
	bool isCharging;
	float attackTime;
	float maxAttackTime;
	float chargeTime;
	int enemyLayerMask;
	AudioClip whileShootingSFX;
	AudioClip onShootSFX;
	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/LaserChargeSFX", typeof(AudioClip)));
		onShootSFX = (AudioClip)(Resources.Load("Sounds/LaserOnShootSFX", typeof(AudioClip)));
		whileShootingSFX = (AudioClip)(Resources.Load("Sounds/LaserFiringSFX", typeof(AudioClip)));

		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Laser", 4f, 2f, 40, 25, 60);

		maxAttackTime = 2.7f;
		chargeTime = 0.4f;
		laser = bulletPointTransform.GetComponent<LineRenderer> ();
		laser.enabled = false;


		enemyLayerMask = LayerMask.GetMask ("Enemies");

	}

	protected override void Attack()
	{
		
		if (isAttacking == false) {
			sourceSFX.PlayOneShot(shootSFX);
			((GameObject)Instantiate (shootParticle, bulletPointTransform.position, shootParticle.transform.rotation)).transform.SetParent (bulletPointTransform);
			isAttacking = true;
			StartCoroutine ("FireLaser");

		} 

	}


	IEnumerator FireLaser()
	{
		attackTime = 0f;

		while (isAttacking) {

			attackTime += Time.fixedDeltaTime;

			if (attackTime >= maxAttackTime && sourceSFX.isPlaying == false) {
				laser.enabled = false;
				isAttacking = false;
				timeUntilAttack = attackDelay;

			} else if (attackTime >= chargeTime) {
				if (!laser.enabled) {
					sourceSFX.PlayOneShot(onShootSFX);
					laser.enabled = true;
					sourceSFX.PlayOneShot (whileShootingSFX);
				}

				if (sourceSFX.isPlaying == false) sourceSFX.PlayOneShot (whileShootingSFX);

				laser.material.mainTextureOffset = new Vector2 (-Time.time, 0);


				Vector3 shootDirection = currentTargetT.position + Vector3.up * 0.5f - bulletPointTransform.position;

				Ray ray = new Ray (bulletPointTransform.position, shootDirection);
				RaycastHit[] hit;

				laser.SetPosition (0, ray.origin);

				if (!targetIsDead() && currentTarget != null) {
					laser.SetPosition (1, ray.GetPoint (100f));
				} else {
					laser.SetPosition (1, ray.GetPoint (0f));
				}


				hit = Physics.SphereCastAll (bulletPointTransform.position, 0.5f, shootDirection, 100f, enemyLayerMask);

				for (int i = 0; i < hit.Length; i++) {

					if (hit[i].transform.CompareTag ("Enemy")) {
						Unit currentUnit = hit[i].transform.GetComponent<Unit> ();
						currentUnit.Damage (attackDamage * Time.fixedDeltaTime);
					}
				}
			}

			yield return null;
		}


	}


}
