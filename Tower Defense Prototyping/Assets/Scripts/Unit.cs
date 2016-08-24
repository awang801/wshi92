using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	//From Dead Earth Tutorial
	//**************************************************************
	//Inspector Assigned Variables
	public bool HasPath = false;
	public bool PathPending = false;
	public bool PathStale = false;
	public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

	//Private Variables
	NavMeshAgent navAgent = null;
	Animator animator = null;

	int angleHash;
	int speedHash;
	int deathHash;
	int finishHash;

	float smoothAngle = 0f;
	public bool MixedMode = false;

	//**************************************************************
	float health;

	public Transform target;
	NavMeshPath path;
	NavMeshPath calcPath;

	public GameObject attackPlayer;
	public GameObject sendPlayer;

	public bool isDying;

	bool ableToFind;
	bool calculating;

	Bank bank;

	MouseFunctions mFunc;

	void Awake()
	{
		mFunc = GameObject.Find("GameManager").GetComponent<MouseFunctions>();
		navAgent = GetComponent<NavMeshAgent> ();
		animator = GetComponent<Animator> ();
		angleHash = Animator.StringToHash ("Angle");
		speedHash = Animator.StringToHash ("Speed");
		deathHash = Animator.StringToHash ("Dead");
		finishHash = Animator.StringToHash ("Finish");
		navAgent.updateRotation = false;
	}

	// Use this for initialization
	void Start () {
		
		health = 5;
		bank = attackPlayer.GetComponent<Bank>();

	}

	public void Damage(float dmg)
	{
		health -= dmg;
		if (health <= 0) {
			Death ();
		}
	}

	public void setTarget(Transform _target)
	{
		target = _target;
		navAgent.SetDestination (target.position);
	}

	void Death()
	{
		isDying = true;
		animator.SetTrigger (deathHash);
		//Instantiate (Resources.Load ("Enemies/EnemyDeath"), transform.position, transform.rotation);
		Destroy (gameObject, animator.GetCurrentAnimatorClipInfo(0).Length + 1);
        bank.addMoney(15);
    }

    public void Finish()
    {
		isDying = true;
		animator.SetTrigger (finishHash);
        //Instantiate(Resources.Load("Enemies/EnemyDeath"), transform.position, transform.rotation); //Play some sort of teleport animation here
		Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0).Length);
    }

	void OnAnimatorMove() //Access to Root Motion 
	{
		if (MixedMode && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Locomotion")) {
			transform.rotation = animator.rootRotation;
		}

		navAgent.velocity = animator.deltaPosition / Time.deltaTime;
	}

	//FROM OLD ENEMY SCRIPT, MIGHT NOT NEED
	void Update()
    {
		//FROM DEAD EARTH TUTORIAL
		//**************************************************************************************
		HasPath = navAgent.hasPath;
		PathPending = navAgent.pathPending;
		PathStale = navAgent.isPathStale;
		PathStatus = navAgent.pathStatus;

		Vector3 localDesiredVelocity = transform.InverseTransformVector (navAgent.desiredVelocity);
		float angle = Mathf.Atan2 (localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;
		smoothAngle = Mathf.MoveTowardsAngle (smoothAngle, angle, 80.0f * Time.deltaTime);

		float speed = localDesiredVelocity.z;

		animator.SetFloat (angleHash, smoothAngle);
		animator.SetFloat (speedHash, speed, 0.1f, Time.deltaTime);

		if (navAgent.desiredVelocity.sqrMagnitude > Mathf.Epsilon) {
			if (!MixedMode || (MixedMode && Mathf.Abs(angle) < 80f && animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion") )) {
				Quaternion lookRotation = Quaternion.LookRotation (navAgent.desiredVelocity, Vector3.up);
				transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, 5.0f * Time.deltaTime);
			} 
		}
		//**************************************************************************************

		//OLD CODE - Testing how to prevent enemies from stopping everytime a new wall is built
		//======================================================================================

        /*if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("Agent has an incomplete path? " + gameObject);
            navAgent.SetDestination(target.position);
        }
        if (navAgent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Agent has no valid path " + gameObject);
            navAgent.SetDestination(target.position);
        }

        if (navAgent.hasPath == false && path != null)
        {
            navAgent.path = path;
            navAgent.Resume();

            ableToFind = navAgent.CalculatePath(target.position, calcPath);
            calculating = true;
            Debug.Log("CALCULATING NEW PATH, SETTING TEMPORARY");
        }
        else if (path != navAgent.path)
        {
            path = navAgent.path;
        }
        if (calculating == true && navAgent.pathPending == false && ableToFind == true)
        {
            calculating = false;
            navAgent.path = calcPath;

            Debug.Log("**NEW PATH SET!");
        }
        */
    }

}
