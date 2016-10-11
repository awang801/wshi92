using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TowerIce : Tower
{

	bool isAttacking;

	public IceBullet ice;

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/IceSFX", typeof(AudioClip)));

		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Ice", 0.1f, 0.0333f, 40, 25, 60);

		ice = shootParticle.GetComponent<IceBullet> ();

		ice.Setup (attackDamage, 50f);


	}



	protected override void Attack()
	{
		if (isAttacking == false) {
			ice.ToggleOn ();
			isAttacking = true;
			StartCoroutine ("FireIce");
		} 
	}

	IEnumerator FireIce()
	{
		while (isAttacking) {

			if (currentTargetUnit == null || targetIsDying() == true) {

				ice.ToggleOff ();
				isAttacking = false;
				sourceSFX.Stop ();

				if (isServer) RpcStopIce ();

			} else {
				
				if (sourceSFX.isPlaying == false) sourceSFX.Play();
				ice.ToggleOn ();
				isAttacking = true;

			}

			yield return new WaitForSeconds(0.1f);
		}


	}

	[ClientRpc]
	void RpcStopIce()
	{
		ice.ToggleOff ();
		isAttacking = false;
		sourceSFX.Stop ();
	}

}
