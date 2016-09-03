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

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/LaserFireSFX", typeof(AudioClip)));
		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Laser", 4f, 3f, 40, 25, 60);

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
			Instantiate (shootParticle, bulletPointTransform.position, shootParticle.transform.rotation);
			isAttacking = true;
			StartCoroutine ("FireLaser");

		} 

	}


	IEnumerator FireLaser()
	{
		attackTime = 0f;

		while (isAttacking) {

			attackTime += Time.fixedDeltaTime;

			if (attackTime >= maxAttackTime) {
				laser.enabled = false;
				isAttacking = false;
				timeUntilAttack = attackDelay;

			} else if (attackTime >= chargeTime) {
				if (!laser.enabled)	laser.enabled = true;

				laser.material.mainTextureOffset = new Vector2 (-Time.time, 0);

				Vector3 shootDirection = currentTargetT.position + Vector3.up * 0.5f - bulletPointTransform.position;

				Ray ray = new Ray (bulletPointTransform.position, shootDirection);
				RaycastHit[] hit;

				laser.SetPosition (0, ray.origin);
				laser.SetPosition (1, ray.GetPoint (100f));


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
