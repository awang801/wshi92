using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Drain : MonoBehaviour {

	public List<GameObject> unitsInRange;
	public GameObject drainParticle;
	public GameObject myVampireObj;
	public Vampire myVampire;

	GameObject currentTarget;
	Unit currentTargetUnit;

	int maxDrainNumber = 5;

	string OwnerPlayerId;

	public NetworkIdentity myUnitId;

	public void CastDrain()
	{
		if (!myUnitId.isServer)
			return;
		
		int counter = 0;

		foreach (var unit in unitsInRange)
		{
			if (unit != null) {
				Unit currentUnit = unit.GetComponent<Unit> ();

				if (!currentUnit.isDying && !currentUnit.isDead) {

					myVampire.RpcDrainUnit (currentUnit.netId);
					Instantiate (drainParticle, unit.transform, false);
					counter++;

					if (counter >= maxDrainNumber) {
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

		if (!myUnitId.isServer)
			return;


		GameObject newTarget = other.gameObject;

		if (newTarget.GetInstanceID() == myVampireObj.GetInstanceID())
			return;

	
		if (newTarget.CompareTag ("Enemy")) {

			if (!unitsInRange.Contains (newTarget)) {
				unitsInRange.Add (newTarget);
			}

		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!myUnitId.isServer)
			return;

		GameObject newTarget = other.gameObject;

		if (newTarget.CompareTag ("Enemy")) {

			if (unitsInRange.Contains (newTarget)) {
				unitsInRange.Remove (newTarget);
			}

		}
	}
}
