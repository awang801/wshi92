using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceBullet : MonoBehaviour {

	public ParticleSystem particles;
	public ParticleSystem particles2;

	public List<GameObject> unitsInRange;

	public float damage;
	public float coldness;

	public bool attacking;

	public int everyXFrames = 3;
	public int frameCounter = 0;

	public void Setup(float dmg, float cold)
	{
		damage = dmg;
		coldness = cold;
		ToggleOff ();

	}

	void Update()
	{
		if (frameCounter < everyXFrames) {
			frameCounter += 1;
		} else {
			frameCounter = 0;
			if (attacking) {
				DamageUnitsInTrigger ();
			}
		}

	}

	public void ToggleOn()
	{
		if (!particles.isPlaying) {
			particles.Play ();
			particles2.Play ();
			attacking = true;
		}

	}

	public void ToggleOff()
	{
		if (particles.isPlaying) {
			particles.Stop ();
			particles2.Stop ();
			attacking = false;
		}
	}

	void DamageUnitsInTrigger()
	{
		foreach (var unit in unitsInRange)
		{
			if (unit != null) {
				Unit currentUnit = unit.GetComponent<Unit> ();
				if (!currentUnit.isDying) {
					currentUnit.Damage (damage * Time.deltaTime * everyXFrames);
					currentUnit.addTemperature (-coldness * Time.deltaTime * everyXFrames);
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{

		GameObject newTarget = other.gameObject;

		if (newTarget.CompareTag ("Enemy")) {

			if (!unitsInRange.Contains (newTarget)) {
				unitsInRange.Add (newTarget);
			}

		}
	}

	void OnTriggerExit(Collider other)
	{
		GameObject newTarget = other.gameObject;

		if (newTarget.CompareTag ("Enemy")) {

			if (unitsInRange.Contains (newTarget)) {
				unitsInRange.Remove (newTarget);
			}

		}
	}

}
