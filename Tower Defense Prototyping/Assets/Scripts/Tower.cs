using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Tower : MonoBehaviour
{

	//Inspector Set Variables ============================================
	public GameObject bullet;

	//FUNCTIONALITY =======================================================

	//Attack Variables
	float attackRange; //References the Radius of the sphere collider attached to tower
	float attackDamage; //Attack DMG
	float attackDelay; //Attack Delay in Seconds
	float timeSinceAttack; //Tracker used in conjunction with attackDelay
	float cost;
	float upgradeCost;
	int sellValue;



	float rotationSpeed; //How fast the tower rotates to a new target (will track the target afterwards)
	bool recentNewTarget;

	Unit currentTargetUnit;
	Transform currentTargetT;
	public List<GameObject> unitsInRange;

	//AUDIO =================================================================
	protected AudioClip shootSFX;
	AudioSource sourceSFX;

	//ANIMATION ==============================================================
	Animator animator;
	int shootHash;
	GameObject currentTarget;
	GameObject rotatePart;
	Transform bulletPointTransform;
	Transform rotatePartTransform;

	//REFERENCES 
	public Node myNode;


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
		animator = gameObject.GetComponent<Animator> ();
		shootHash = Animator.StringToHash ("Shoot");
		attackRange = this.gameObject.GetComponent<SphereCollider>().radius * this.transform.lossyScale.x * 100f;

		rotationSpeed = 10f;

		sourceSFX = this.gameObject.GetComponent<AudioSource>();

		rotatePartTransform = gameObject.transform.GetChild(0);
		bulletPointTransform = rotatePartTransform.GetChild(1);

		unitsInRange = new List<GameObject>();
	}

	void Update()
	{

		timeSinceAttack += Time.deltaTime;

		if (currentTarget != null) {
			if (targetIsDead () == false) {

				Vector3 targetNoYAxis = currentTargetT.position;
				targetNoYAxis.y = rotatePartTransform.position.y;
				rotatePartTransform.LookAt(targetNoYAxis, Vector3.up);

				if (timeSinceAttack >= attackDelay && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base_Layer.Shooting")) {
					Attack ();
					animator.SetTrigger(shootHash);
				} 


			} else {
				findNewTarget ();
			}
		} else {
			findNewTarget ();
		}

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

		newBullet.setup(currentTargetUnit, attackDamage, 25f);

		timeSinceAttack = 0;

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

	void SlowRotateZ()
	{
		Vector3 relativePos = currentTargetT.position - rotatePartTransform.position;
		relativePos.y = gameObject.transform.position.y - 0.3f;
		Debug.Log (rotatePartTransform.position.y);
		Quaternion toRotation = Quaternion.LookRotation(relativePos);
		rotatePartTransform.rotation = Quaternion.Lerp(rotatePartTransform.rotation, toRotation, 0.2f);

		float angle = Quaternion.Angle(rotatePartTransform.rotation, toRotation);

		if (angle < 5f)
		{
			recentNewTarget = false;
		}

	}*/

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
