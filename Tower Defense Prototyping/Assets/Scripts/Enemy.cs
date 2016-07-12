using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int range = 500;
	public Transform target;
	NavMeshAgent agent;

	MouseFunctions gmMFunc;




	void Start()
	{
		gmMFunc = GameObject.Find ("GameManager").GetComponent<MouseFunctions>();
		target = GameObject.Find ("Destination").transform;
		agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (target.position);
		agent.transform.LookAt (target);
	}

	void Update()
	{
		
	}

}