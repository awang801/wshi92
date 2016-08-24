using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerOrb : MonoBehaviour
{

    AudioClip shootSFX;
    AudioSource sourceSFX;
    float attackRange; //References the Radius of the sphere collider attached to tower

    float attackDamage; //Attack DMG
    float attackDelay; //Attack Delay in Seconds

    float rotationSpeed; //How fast the tower rotates to a new target (will track the target afterwards)

    float timeSinceAttack; //Tracker used in conjunction with attackDelay

    public GameObject bullet;

    bool recentNewTarget;

	Animator animator;
	int shootingHash;

    GameObject currentTarget;
    GameObject rotatePart;
    Transform rotatePartTransform;

    Unit currentTargetUnit;
    Transform currentTargetT;

	public List<GameObject> unitsInRange;


    void Awake()
    {
		animator = gameObject.GetComponent<Animator> ();
		shootingHash = Animator.StringToHash ("Shooting");
        attackRange = this.gameObject.GetComponent<SphereCollider>().radius;
		attackDamage = 1f;

		attackDelay = 1.5f;

		rotationSpeed = 10f;

		shootSFX = (AudioClip)(Resources.Load("Sounds/shoot", typeof(AudioClip)));
		sourceSFX = this.gameObject.GetComponent<AudioSource>();

		rotatePartTransform = gameObject.transform.GetChild(0);

		unitsInRange = new List<GameObject>();
    }


    // Update is called once per frame
    void Update()
    {

        timeSinceAttack += Time.deltaTime;

		if (currentTarget != null) {
			if (targetIsDead () == false) {
				if (recentNewTarget) {
					SlowRotateZ();
				} else {
					Vector3 targetNoYAxis = currentTargetT.position;
					targetNoYAxis.y = rotatePartTransform.position.y;
					rotatePartTransform.LookAt(targetNoYAxis, Vector3.up);

					if (timeSinceAttack >= attackDelay) {
						Attack ();
						animator.SetBool (shootingHash, true);
					} else {
						if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Base_Layer.Shooting")) {
							animator.SetBool (shootingHash, false);
						}
					}

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

			/*if (currentTarget == null && other.gameObject.CompareTag("Enemy") && other.GetComponent<Unit>().isDying == false)
			{
				recentNewTarget = true;
				currentTarget = other.gameObject;
				currentTargetUnit = currentTarget.GetComponent<Unit>();
				currentTargetT = currentTarget.transform;
				//Debug.Log ("Current Target changed to : " + currentTarget);
			}*/
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
					break;
				}
			}
		} else {
			animator.SetBool (shootingHash, false); //No Target
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

    void Attack()
    {
        Bullet newBullet = ((GameObject)(Instantiate(bullet, rotatePartTransform.position, Quaternion.identity))).GetComponent<Bullet>();

        newBullet.setup(currentTargetUnit, attackDamage, 25f);

        timeSinceAttack = 0;

        sourceSFX.PlayOneShot(shootSFX);
    }

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
		relativePos.y = rotatePartTransform.position.y;
		Quaternion toRotation = Quaternion.LookRotation(relativePos);
		rotatePartTransform.rotation = Quaternion.Lerp(rotatePartTransform.rotation, toRotation, 0.2f);

		float angle = Quaternion.Angle(rotatePartTransform.rotation, toRotation);

		if (angle < 5f)
		{
			recentNewTarget = false;
		}

	}
}
