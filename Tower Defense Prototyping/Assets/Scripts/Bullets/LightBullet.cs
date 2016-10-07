using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBullet : Bullet
{

	public float maxDamageTime = 2f;
	public float lifeTime = 2.5f;

	float chargeTime = 0.6f;
	float timeElapsed;

	public List<GameObject> unitsInRange;

	public int everyXFrames = 3;
	public int frameCounter = 0;

    protected override void Update()
    {
       
		timeElapsed += Time.deltaTime;

		if (timeElapsed >= lifeTime) {
			
			Destroy (gameObject);

		} else if (timeElapsed >= chargeTime && timeElapsed <= maxDamageTime) {

			if (frameCounter < everyXFrames) {
				frameCounter += 1;
			} else {
				frameCounter = 0;
				DamageUnitsInTrigger ();
			}
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

			currentUnit.Damage (damage * Time.deltaTime * everyXFrames);

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
