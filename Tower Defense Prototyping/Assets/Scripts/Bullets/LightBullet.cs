using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBullet : Bullet
{

	public float maxDamageTime = 2f;
	public float lifeTime = 2.5f;
	float maxDist = 1f;

	float chargeTime = 0.6f;
	float timeElapsed;

	public List<GameObject> unitsInRange;

    protected override void FixedUpdate()
    {
       
		timeElapsed += Time.fixedDeltaTime;

		if (timeElapsed >= lifeTime) {
			Destroy (gameObject);
		} else if (timeElapsed >= chargeTime && timeElapsed <= maxDamageTime) {
			DamageUnitsInTrigger ();
		}
        
    }

	public void Setup (float dam)
	{
		damage = dam;
	}

	void DamageUnitsInTrigger()
	{
		foreach (var unit in unitsInRange)
		{
			Unit currentUnit = unit.GetComponent<Unit> ();
			float dist = Vector3.Distance (unit.transform.position, transform.position);

			currentUnit.Damage (damage * Time.fixedDeltaTime * (dist / maxDist));

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
