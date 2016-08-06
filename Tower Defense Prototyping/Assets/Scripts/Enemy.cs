using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int range = 500;
	public Transform target;
	NavMeshAgent agent;
	NavMeshPath path;
	NavMeshPath calcPath;

	bool ableToFind;
	bool calculating;

	MouseFunctions mFunc;


	void Awake()
	{
		mFunc = GameObject.Find ("GameManager").GetComponent<MouseFunctions> ();
	}

	void Start()
	{
		
		target = GameObject.Find ("Destination").transform;

		agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (target.position);
		agent.transform.LookAt (target);

		calcPath = new NavMeshPath();

	}

	void Update()
	{
		if (agent.pathStatus == NavMeshPathStatus.PathInvalid) {
			Debug.LogWarning ("Agent has an incomplete path? " + gameObject);
			agent.SetDestination (target.position);
		}
		if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
			Debug.LogWarning ("Agent has no valid path " + gameObject);
			agent.SetDestination (target.position);
		}

		if (agent.hasPath == false && path != null) {
			agent.path = path;
			agent.Resume ();

			ableToFind = agent.CalculatePath (target.position, calcPath);
			calculating = true;
			Debug.Log ("CALCULATING NEW PATH, SETTING TEMPORARY");
		} else if (path != agent.path) {
			path = agent.path;
		}
		if (calculating == true && agent.pathPending == false && ableToFind == true) {
			calculating = false;
			agent.path = calcPath;

			Debug.Log ("**NEW PATH SET!");
		}
	}

}