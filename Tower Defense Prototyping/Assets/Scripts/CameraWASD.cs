using UnityEngine;
using System.Collections;

public class CameraWASD : MonoBehaviour {


	float xMovement;
	float yMovement;
	float mouseWheel;

	Vector3 move;

	float moveSpeed = 40f;
	// Use this for initialization
	void Awake() {


	}


	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		xMovement = Input.GetAxisRaw ("Horizontal");
		yMovement = Input.GetAxisRaw ("Vertical");
		mouseWheel = Input.GetAxis ("Mouse ScrollWheel");


		move = new Vector3 (xMovement, yMovement * Mathf.Sin(20), mouseWheel + yMovement * Mathf.Cos(20)).normalized;


	}

	void FixedUpdate()
	{
		this.gameObject.transform.Translate (move * moveSpeed * Time.deltaTime);

	}
}
