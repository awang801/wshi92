using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HealBot : MonoBehaviour {

	public List<GameObject> unitsInRange;
	public GameObject resurrectedParticle;
	public GameObject myMercyObj;
	public Mercy myMercy;

	GameObject currentTarget;
	Unit currentTargetUnit;

	int maxResNumber = 5;

	string OwnerPlayerId;

	public NetworkIdentity myUnitId;

	public void CastResurrect()
	{
		if (!myUnitId.isServer)
			return;
		
		int counter = 0;

		foreach (var unit in unitsInRange)
		{
			if (unit != null) {
				Unit currentUnit = unit.GetComponent<Unit> ();

				if (currentUnit.isDying && !currentUnit.isDead && !currentUnit.isBeingResurrected && !currentUnit.attackPlayer.name.Equals(OwnerPlayerId)) {

					myMercy.RpcResurrectUnit (currentUnit.netId);
					counter++;

					if (counter >= maxResNumber) {
						break;
					}
				}

			}
		}

	}

	public void ResurrectUnit(Unit resUnit)
	{

		resUnit.ResurrectMe ();
		Instantiate (resurrectedParticle, resUnit.transform, false);

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

		if (newTarget.GetInstanceID() == myMercyObj.GetInstanceID())
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
