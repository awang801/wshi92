using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Tower : MonoBehaviour
{

	//Inspector Set Variables ============================================
	public GameObject bullet;
	public GameObject shootParticle;

	//FUNCTIONALITY =======================================================

	//Attack Variables
	protected float attackRange; //References the Radius of the sphere collider attached to tower
	protected float attackDamage; //Attack DMG
	protected float attackDelay; //Attack Delay in Seconds
	protected float timeUntilAttack; //Tracker used in conjunction with attackDelay
	float cost;
	float upgradeCost;
	int sellValue;



	float rotationSpeed; //How fast the tower rotates to a new target (will track the target afterwards)
	bool recentNewTarget;

	protected Unit currentTargetUnit;
	protected Transform currentTargetT;
	public List<GameObject> unitsInRange;

	//AUDIO =================================================================
	protected AudioClip shootSFX;
	protected AudioClip buildSFX;
	protected AudioSource sourceSFX;


	//ANIMATION ==============================================================
	Animator animator;
	int shootHash;
	protected GameObject currentTarget;
	protected GameObject rotatePart;
	protected Transform bulletPointTransform;
	protected Transform rotatePartTransform;

	bool isBeingBuilt;
	Vector3 buildAt;

	//REFERENCES 
	public Node node;


	protected void setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
	{
		name = _name;
		attackDamage =_adamage;
		attackDelay = _adelay;
		cost = _cost;
		sellValue = _sellvalue;
		upgradeCost = _upcost;
	}

	protected virtual void Awake()
	{
		isBeingBuilt = true;

		animator = gameObject.GetComponent<Animator> ();
		shootHash = Animator.StringToHash ("Shoot");
		attackRange = this.gameObject.GetComponent<SphereCollider>().radius * this.transform.lossyScale.x * 100f;

		rotationSpeed = 10f;

		sourceSFX = this.gameObject.GetComponent<AudioSource>();

		buildSFX = (AudioClip)(Resources.Load("Sounds/siegemode", typeof(AudioClip)));

		rotatePartTransform = gameObject.transform.GetChild(0);
		bulletPointTransform = rotatePartTransform.GetChild(1);

		unitsInRange = new List<GameObject>();

		sourceSFX.PlayOneShot (buildSFX);
	}

	void Update()
	{
		if (!isBeingBuilt) {
			timeUntilAttack -= Time.deltaTime;

			if (currentTarget != null) {
				if (targetIsDead () == false) {

					Vector3 targetNoYAxis = currentTargetT.position;
					targetNoYAxis.y = rotatePartTransform.position.y;
					rotatePartTransform.LookAt (targetNoYAxis, Vector3.up);

					if (timeUntilAttack <= 0 && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base_Layer.Shooting")) {
						Attack ();
						animator.SetTrigger (shootHash);
					} 


				} else {
					findNewTarget ();
				}
			} else {
				findNewTarget ();
			}
		} else {

		}
	}

	public void startTargetAnimationPoint(Vector3 moveTo)
	{
		buildAt = moveTo;

		StartCoroutine (buildAnimation());
	}

	IEnumerator buildAnimation()
	{
		while (Vector3.Distance (transform.position, buildAt) > 0.05) {

			transform.position = Vector3.Lerp (transform.position, buildAt, 4*Time.deltaTime);

			yield return new WaitForSeconds (Time.deltaTime);
		}

		transform.position = buildAt;
		isBeingBuilt = false;
	}

	void OnTriggerEnter(Collider other)
	{

		GameObject newTarget = other.gameObject;

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
			if (currentTarget == newTarget) {
				currentTarget = null;
				findNewTarget ();
			}

		}
	}

	void findNewTarget()
	{
		if (unitsInRange.Count > 0) {
			foreach (var unit in unitsInRange) {
				if (unit == null) {
					unitsInRange.Remove (unit);
				} else {
					currentTargetUnit = unit.GetComponent<Unit> ();
					currentTarget = unit;
					currentTargetT = currentTarget.transform;
					recentNewTarget = true;
					break;
				}
			}
		} 
	}

	bool targetIsDead()
	{
		if (currentTargetUnit.isDying == true) {
			unitsInRange.Remove (currentTarget);
			return true;
		} else {
			return false;
		}

	}

	protected virtual void Attack()
	{
		Bullet newBullet = ((GameObject)(Instantiate(bullet, bulletPointTransform.position, Quaternion.identity))).GetComponent<Bullet>();
		Instantiate (shootParticle, bulletPointTransform.position, bulletPointTransform.rotation);
		newBullet.Setup(currentTargetUnit, attackDamage, 25f);

		timeUntilAttack = attackDelay;

		sourceSFX.PlayOneShot(shootSFX);
	}

	/*
	void SlowRotate()
	{
		Vector3 relativePos = currentTargetT.position - rotatePartTransform.position;
		Quaternion toRotation = Quaternion.LookRotation(relativePos);
		rotatePartTransform.rotation = Quaternion.Lerp(rotatePartTransform.rotation, toRotation, 0.2f);

		float angle = Quaternion.Angle(rotatePartTransform.rotation, toRotation);

		if (angle < 5f)
		{
			recentNewTarget = false;
		}

	}
*/
	void SlowRotateZ()
	{
		Vector3 relativePos = currentTargetT.position - rotatePartTransform.position;
		relativePos.y = rotatePartTransform.position.y;
		//Debug.Log (rotatePartTransform.position.y);
		Quaternion toRotation = Quaternion.LookRotation(relativePos);
		rotatePartTransform.rotation = Quaternion.Lerp(rotatePartTransform.rotation, toRotation, 0.2f);

		float angle = Quaternion.Angle(rotatePartTransform.rotation, toRotation);

		if (angle < 5f)
		{
			recentNewTarget = false;
		}

	}
		

	public string[] Stats
	{
		get
		{
			string[] stats = new string[7];
			stats [0] = name;
			stats [1] = attackDamage.ToString();
			stats [2] = attackRange.ToString();
			stats [3] = (1/attackDelay).ToString("F2");
			stats [4] = cost.ToString();
			stats [5] = sellValue.ToString();
			stats [6] = upgradeCost.ToString();
			return stats;
		}
	}

}
