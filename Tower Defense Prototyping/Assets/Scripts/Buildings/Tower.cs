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

	//float rotationSpeed; //How fast the tower rotates to a new target (will track the target afterwards)
	//bool recentNewTarget;

	protected Unit currentTargetUnit;
	protected Transform currentTargetT;
	public List<GameObject> unitsInRange;
	public List<GameObject> dyingUnitsInRange;

	//AUDIO =================================================================
	protected AudioClip shootSFX;
	protected AudioClip buildSFX;
	protected AudioSource sourceSFX;


	//ANIMATION =============================================================
	Animator animator;
	int shootHash;

	protected Transform modelParts;
	protected Transform scriptParts;

	protected GameObject currentTarget;
	protected GameObject rotatePart;
	protected Transform bulletPointTransform;
	protected Transform rotatePartTransform;

	public bool isBeingBuilt;

	public Vector3 buildAt;


	//REFERENCES 
	public Node node;
	public string OwnerPlayerId;

	//OWNERSHIP ===================================================================



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

		//rotationSpeed = 10f;

		sourceSFX = this.gameObject.GetComponent<AudioSource>();

		buildSFX = (AudioClip)(Resources.Load("Sounds/siegemode", typeof(AudioClip)));

		modelParts = gameObject.transform.GetChild (0);
		scriptParts = gameObject.transform.GetChild (1);

		rotatePartTransform = modelParts.GetChild(0);
		bulletPointTransform = rotatePartTransform.GetChild (0);

		unitsInRange = new List<GameObject>();
		dyingUnitsInRange = new List<GameObject> ();

		sourceSFX.PlayOneShot (buildSFX);

		StartCoroutine(startTargetAnimationPoint (transform.position + Vector3.up * 2f, 1.5f));
	}

	protected virtual void Update()
	{
		if (!isBeingBuilt) {

			if (isServer) {
				timeUntilAttack -= Time.deltaTime;
			}


			if (currentTarget != null) {
				if (targetIsDying () == false) {

					Vector3 targetNoYAxis = currentTargetT.position;
					targetNoYAxis.y = rotatePartTransform.position.y;
					rotatePartTransform.LookAt (targetNoYAxis, Vector3.up);


					if (isServer) {
						if (timeUntilAttack <= 0) {

							RpcAttack ();

						} 
					}


				} else {

					currentTarget = null;

					if (isServer) {
						findNewTarget ();
					}


				}
			} else {

				if (isServer) {
					findNewTarget ();
				}

			}
		}
	}



	void OnTriggerEnter(Collider other)
	{

		if (!isServer) {
			return;
		}
			GameObject newTarget = other.gameObject;

			if (newTarget.CompareTag ("Enemy")) {			
				if (!unitsInRange.Contains (newTarget)) {
					unitsInRange.Add (newTarget);
				}

			}


	}

	void OnTriggerExit(Collider other)
	{
		if (!isServer) {
			return;
		}

		GameObject newTarget = other.gameObject;

		if (newTarget.CompareTag ("Enemy")) {

			if (unitsInRange.Contains (newTarget)) {
				unitsInRange.Remove (newTarget);
			}

			if (dyingUnitsInRange.Contains (newTarget)) {
				dyingUnitsInRange.Remove (newTarget);
			}

			if (currentTarget != null) {
				if (currentTarget == newTarget) {
					currentTarget = null;
					findNewTarget ();
				}
			}

		}
	}

	[ClientRpc]
	void RpcSetTarget(NetworkInstanceId targetID)
	{
		
		if (isServer) { //Not sure if this is needed, does the server client also get called during a Rpc?
			currentTarget = NetworkServer.FindLocalObject (targetID);
		} else if (isClient) {
			currentTarget = ClientScene.FindLocalObject (targetID);
		}

		currentTargetUnit = currentTarget.GetComponent<Unit> ();
		currentTargetT = currentTarget.transform;

	}

	void findNewTarget()
	{
		if (unitsInRange.Count > 0) {
			
			for (int i = 0; i < unitsInRange.Count; i++) {
				GameObject unit = unitsInRange [i];

				if (unit == null) {
					unitsInRange [0] = null;
					unitsInRange.Remove (unit);
				} else {
					currentTargetUnit = unit.GetComponent<Unit> ();
					if (currentTargetUnit.sendPlayer.name != OwnerPlayerId) {

						currentTarget = unit;
						currentTargetT = currentTarget.transform;

						NetworkIdentity newTargetID = currentTarget.GetComponent<NetworkIdentity> ();
						RpcSetTarget (newTargetID.netId);

						//recentNewTarget = true;
						break;

					}
				}
			}

		} else {

			if (dyingUnitsInRange.Count > 0) {
				for (int i = 0; i < dyingUnitsInRange.Count; i++) {
					
					GameObject dyingUnit = dyingUnitsInRange [i];

					if (dyingUnit == null) {
						dyingUnitsInRange.Remove (dyingUnit);
					} else {
						Unit TempTargetUnit = dyingUnit.GetComponent<Unit> ();
						if (TempTargetUnit.sendPlayer.name != OwnerPlayerId) {

							if (TempTargetUnit.isDead) {
								dyingUnitsInRange.Remove (dyingUnit);
								continue;
							}
							else if (TempTargetUnit.isDying == false) {
								unitsInRange.Add (dyingUnit);
								dyingUnitsInRange.Remove (dyingUnit);
								currentTargetUnit = TempTargetUnit;
								currentTarget = dyingUnit;
								currentTargetT = currentTarget.transform;

								NetworkIdentity newTargetID = currentTarget.GetComponent<NetworkIdentity> ();
								RpcSetTarget (newTargetID.netId);
								//recentNewTarget = true;
								break;
							}
						}
					}

				}
			}

		}
	}

	protected bool targetIsDying()
	{
		if (currentTarget != null) {
			if (currentTargetUnit.isDying == true) {
				unitsInRange.Remove (currentTarget);
				dyingUnitsInRange.Add (currentTarget);
				return true;
			} else {
				return false;
			}
		} else {
			return true;
		}
	}

	[ClientRpc]
	protected void RpcAttack()
	{
		if (animator != null) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Base_Layer.Shooting"))
			{
				animator.SetTrigger (shootHash);
			}
		}

		Attack ();
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
			//recentNewTarget = false;
		}

	}
		
	public IEnumerator startTargetAnimationPoint(Vector3 moveTo, float _timedelay)
	{
		yield return new WaitForSeconds (_timedelay);

		StartCoroutine (buildAnimation(moveTo));
	}

	IEnumerator buildAnimation(Vector3 buildAt)
	{

		while (Vector3.Distance (transform.position, buildAt) > 0.05) {

			transform.position = Vector3.Lerp (transform.position, buildAt, 4 * Time.deltaTime);

			yield return new WaitForSeconds (Time.deltaTime);
		}

		transform.position = buildAt;
		isBeingBuilt = false;
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
