using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
			other.gameObject.GetComponent<Unit> ().Damage (100f);

		} 
	}
}
