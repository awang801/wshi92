using UnityEngine;
using System.Collections;

public class UnitMove : MonoBehaviour {

	public Transform startWP;
	public Transform endWP;

	public Transform[] waypoints;
	public Transform currentTarget;

	float minDistance = 3f;

	Vector3 moveDirection;

	float moveSpeed = 2.5f;

	int indexWP = 0;

	// Use this for initialization
	void Awake () {
		currentTarget = endWP;
	}
	
	// Update is called once per frame
	void Update () {
		moveDirection = (currentTarget.position - transform.position).normalized;
	}

	void FixedUpdate() {


		if (Vector3.Distance (transform.position, currentTarget.position) > minDistance) {
			
			transform.Translate (moveDirection * moveSpeed * Time.deltaTime);

		} else {

			indexWP++;

			if (indexWP >= waypoints.Length) {
				indexWP = 0;
			}

			currentTarget = waypoints [indexWP];
			Debug.Log (waypoints [indexWP]);

		}
	}

	public void Setup(Transform[] wps, int startIndex, float moveSpd)
	{
		waypoints = wps;
		currentTarget = waypoints [startIndex];
		moveSpeed = moveSpd;
	}
		
}
