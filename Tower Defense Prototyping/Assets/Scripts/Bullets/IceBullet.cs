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

	public void Setup(float dmg, float cold)
	{
		damage = dmg;
		coldness = cold;
		ToggleOff ();

	}

	void Update()
	{
		if (attacking) {
			DamageUnitsInTrigger ();
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
					currentUnit.Damage (damage * Time.deltaTime);
					currentUnit.addTemperature (-coldness * Time.deltaTime);
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
