using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	AudioClip shootSFX;
	AudioSource sourceSFX;
	float attackRange; //References the Radius of the sphere collider attached to tower

	float attackDamage; //Attack DMG
	float attackDelay; //Attack Delay in Seconds

	float rotationSpeed; //How fast the tower rotates to a new target (will track the target afterwards)

	float timeSinceAttack; //Tracker used in conjunction with attackDelay

	public GameObject bullet;

	bool recentNewTarget;

	GameObject currentTarget;
	Unit currentTargetUnit;
	Transform currentTargetT;


	void Awake()
	{
		attackRange = this.gameObject.GetComponent<SphereCollider> ().radius;
	}

	void Start () {
		attackDamage = 1f;

		attackDelay = 1f;

		rotationSpeed = 10f;

		shootSFX = (AudioClip)(Resources.Load ("shoot", typeof(AudioClip)));
		sourceSFX = this.gameObject.GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

		timeSinceAttack += Time.deltaTime;

		if (currentTarget != null) 
		{
				
			if (recentNewTarget) {
				SlowRotate ();

			} 
			else {
				transform.LookAt (currentTarget.transform);
			
				if (timeSinceAttack >= attackDelay) {
					Attack ();
				}
			}

		}

	}





	void OnTriggerStay(Collider other)
	{
		if (currentTarget == null && other.gameObject.CompareTag ("Enemy")) {
			recentNewTarget = true;
			currentTarget = other.gameObject;
			currentTargetUnit = currentTarget.GetComponent<Unit> ();
			currentTargetT = currentTarget.transform;
			//Debug.Log ("Current Target changed to : " + currentTarget);
		} 
	}

	void OnTriggerExit(Collider other)
	{
		if (currentTarget == other.gameObject) {
			currentTarget = null;
			//Debug.Log ("Current Target Left Range");
		}
	}

	void Attack()
	{
		Bullet newBullet = ((GameObject)(Instantiate (bullet, transform.position, Quaternion.identity))).GetComponent<Bullet>();

		newBullet.setup (currentTargetUnit, attackDamage, 25f);

		timeSinceAttack = 0;

		sourceSFX.PlayOneShot (shootSFX);
	}
		
	void SlowRotate()
	{
		Vector3 relativePos = currentTargetT.position - transform.position;
		Quaternion toRotation = Quaternion.LookRotation (relativePos);
		transform.rotation = Quaternion.Lerp (transform.rotation, toRotation, 0.2f);

		float angle = Quaternion.Angle (transform.rotation, toRotation);

		if (angle < 5f) {
			recentNewTarget = false;
		}

	}
}
