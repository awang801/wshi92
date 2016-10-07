using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonBullet : Bullet
{

	public List<GameObject> unitsInRange;

	public GameObject explosionParticle;


    protected override void Update()
    {
        
			transform.LookAt (targetPosition);
			transform.Translate(Vector3.forward * velocity * Time.deltaTime);

		if (Vector3.Distance(targetPosition, transform.position) < 1f)
        {
			Instantiate (explosionParticle, targetPosition, explosionParticle.transform.rotation);
			DamageUnitsInTrigger ();
            Destroy(gameObject);
        }
        
    }

	void DamageUnitsInTrigger()
	{
		foreach (var unit in unitsInRange)
		{
			if (unit != null) {
				Unit currentUnit = unit.GetComponent<Unit> ();
				currentUnit.Damage (damage);
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
