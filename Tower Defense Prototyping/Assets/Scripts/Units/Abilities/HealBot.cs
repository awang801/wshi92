using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealBot : MonoBehaviour {

	public List<GameObject> unitsInRange;
	public GameObject resurrectedParticle;
	public GameObject myMercy;

	GameObject currentTarget;
	Unit currentTargetUnit;

	int maxResNumber = 5;

	string OwnerPlayerId;


	public void CastResurrect()
	{
		int counter = 0;

		foreach (var unit in unitsInRange)
		{
			if (unit != null) {
				Unit currentUnit = unit.GetComponent<Unit> ();

				if (currentUnit.isDying && !currentUnit.isDead && !currentUnit.isBeingResurrected) {

					currentUnit.ResurrectMe ();
					Instantiate (resurrectedParticle, currentUnit.transform, false);
					counter++;

					if (counter >= maxResNumber) {
						break;
					}
				}

			}
		}

	}

	public void SetOwnerID(string id)
	{
		OwnerPlayerId = id;
	}

	void OnTriggerEnter(Collider other)
	{

		GameObject newTarget = other.gameObject;

		if (newTarget.GetInstanceID() == myMercy.GetInstanceID())
			return;

	
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
