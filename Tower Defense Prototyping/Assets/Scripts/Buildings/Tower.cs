using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public abstract class Tower : NetworkBehaviour
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

	protected Transform modelParts;
	protected Transform scriptParts;

	protected GameObject currentTarget;
	protected GameObject rotatePart;
	protected Transform bulletPointTransform;
	protected Transform rotatePartTransform;

	[SyncVar]
	public bool isBeingBuilt;

	public Vector3 buildAt;


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

		if (animator != null) {
			shootHash = Animator.StringToHash ("Shoot");
		}
		attackRange = this.gameObject.GetComponent<SphereCollider>().radius * this.transform.lossyScale.x * 100f;

		rotationSpeed = 10f;

		sourceSFX = this.gameObject.GetComponent<AudioSource>();

		buildSFX = (AudioClip)(Resources.Load("Sounds/siegemode", typeof(AudioClip)));

		modelParts = gameObject.transform.GetChild (0);
		scriptParts = gameObject.transform.GetChild (1);

		rotatePartTransform = modelParts.GetChild(0);
		bulletPointTransform = rotatePartTransform.GetChild (0);

		unitsInRange = new List<GameObject>();

		sourceSFX.PlayOneShot (buildSFX);
	}

	protected virtual void Update()
	{
		if (!isBeingBuilt) {
			timeUntilAttack -= Time.deltaTime;

			if (currentTarget != null) {
				if (targetIsDead () == false) {

					Vector3 targetNoYAxis = currentTargetT.position;
					targetNoYAxis.y = rotatePartTransform.position.y;
					rotatePartTransform.LookAt (targetNoYAxis, Vector3.up);

					if (timeUntilAttack <= 0) {
						
						if (animator != null) {
							if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Base_Layer.Shooting"))
							{
								animator.SetTrigger (shootHash);
							}
						}
						Attack ();

					} 

				} else {
					findNewTarget ();
				}
			} else {
				findNewTarget ();
			}
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

	protected bool targetIsDead()
	{
		if (currentTarget != null) {
			if (currentTargetUnit.isDying == true) {
				unitsInRange.Remove (currentTarget);
				return true;
			} else {
				return false;
			}
		} else {
			return true;
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
