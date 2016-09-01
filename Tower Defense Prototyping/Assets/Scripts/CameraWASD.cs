using UnityEngine;
using System.Collections;

public class CameraWASD : MonoBehaviour {


	float xMovement;
	float yMovement;
	float mouseWheel;

	float MIN_X = 0f;
	float MAX_X = 50f;
	float MIN_Y = 7f;
	float MAX_Y = 25f;
	float MIN_Z = -4f;
	float MAX_Z = 30f;
	Vector3 move;

	float moveSpeed = 30f;

	void Update () {
		xMovement = Input.GetAxisRaw ("Horizontal");
		yMovement = Input.GetAxisRaw ("Vertical");
		mouseWheel = Input.GetAxis ("Mouse ScrollWheel");

	}

	void FixedUpdate()
	{

		move = (xMovement * Vector3.right + yMovement * Vector3.forward + mouseWheel * -Vector3.up).normalized;

		this.gameObject.transform.Translate (move * moveSpeed * Time.fixedDeltaTime, Space.World);

		transform.position = new Vector3(
			Mathf.Clamp (transform.position.x, MIN_X, MAX_X),
			Mathf.Clamp (transform.position.y, MIN_Y, MAX_Y),
			Mathf.Clamp (transform.position.z, MIN_Z, MAX_Z));

	}
}
