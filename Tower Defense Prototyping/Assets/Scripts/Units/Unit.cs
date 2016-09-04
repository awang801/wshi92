using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	//==============================================================
	//Navmesh Variables
	public bool HasPath = false;
	public bool PathPending = false;
	public bool PathStale = false;
	public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

	public Transform target;
	NavMeshPath path;
	NavMeshPath calcPath;

	NavMeshAgent navAgent = null;
	//==============================================================
	//Animation
	Animator animator = null;

	int angleHash;
	int speedHash;
	int deathHash;
	int finishHash;

	float smoothAngle = 0f;
	public bool MixedMode = false;

	//==============================================================
	//Stats

	float maxHealth;
	float health;

	float baseTemperature = 98.6f;
	float temperature;
	float homeostasisTendency = 0.4f;
	float minTemp = 32f;

	public bool isDying;

	//==============================================================
	//Audio

	AudioSource myAudioSource;
	AudioClip deathSound;
	AudioClip goodSound;
	AudioClip badSound;


	//===============================================================
	//Visuals
	Transform HPBarCanvas;
	Transform HPBar;
	public Quaternion HPBarRotation;

	//===============================================================
	//Owner and References

	public GameObject attackPlayer;
	public GameObject sendPlayer;

	Bank bank;

	MouseFunctions mFunc;

	//==============================================================



	//bool ableToFind;
	//bool calculating;



	void Awake()
	{
		mFunc = GameObject.Find("GameManager").GetComponent<MouseFunctions>();
		navAgent = GetComponent<NavMeshAgent> ();
		animator = GetComponent<Animator> ();
		angleHash = Animator.StringToHash ("Angle");
		speedHash = Animator.StringToHash ("Speed");
		deathHash = Animator.StringToHash ("Dead");
		finishHash = Animator.StringToHash ("Finish");

		HPBarCanvas = transform.GetChild (2);
		HPBar = transform.GetChild (2).GetChild (2);
		HPBarCanvas.Rotate (0, 180, 0);
		HPBarRotation = HPBarCanvas.rotation;
		navAgent.updateRotation = false;



		myAudioSource = GetComponent<AudioSource> ();
		deathSound = (AudioClip)Resources.Load ("Sounds/enemydeath");
		goodSound  = (AudioClip)Resources.Load ("Sounds/GoodBeep");
		badSound  = (AudioClip)Resources.Load ("Sounds/BadBeep");
	}

	// Use this for initialization
	void Start () {
		maxHealth = 5;
		health = 5;
		temperature = baseTemperature;
		bank = attackPlayer.GetComponent<Bank>();

	}

	public void Damage(float dmg)
	{		
		health -= dmg;

		if (health <= 0 && !isDying) {
			HPBar.localScale = new Vector3 (0, 1, 1);
			Death ();
		} else {
			HPBar.localScale = new Vector3 (Mathf.Clamp(health / maxHealth, 0f, 1f), 1, 1);
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
		myAudioSource.PlayOneShot (deathSound);
		//Instantiate (Resources.Load ("Enemies/EnemyDeath"), transform.position, transform.rotation);
		//Destroy (gameObject, animator.GetCurrentAnimatorClipInfo(0).Length + 1);
		//myFader.FadeOut();
		Invoke("temporaryWorkAround", animator.GetCurrentAnimatorClipInfo(0).Length + 1);
        bank.addMoney(15);
    }

	void temporaryWorkAround()
	{
		gameObject.SetActive (false);
	}

    public void Finish()
    {
		isDying = true;
		animator.SetTrigger (finishHash);
		if (attackPlayer.name == "Player 2") {
			myAudioSource.PlayOneShot (goodSound);
		} else {
			myAudioSource.PlayOneShot (badSound);
		}
		//myFader.FadeOut();
		Invoke("temporaryWorkAround", animator.GetCurrentAnimatorClipInfo(0).Length + 1);
        //Instantiate(Resources.Load("Enemies/EnemyDeath"), transform.position, transform.rotation); //Play some sort of teleport animation here
		//Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0).Length);
    }

	void OnAnimatorMove() //Access to Root Motion 
	{
		if (MixedMode && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Locomotion")) {
			transform.rotation = animator.rootRotation;
		}

		navAgent.velocity = animator.deltaPosition / Time.deltaTime;
	}

	public void addTemperature(float amount)
	{
		if (!(amount < 0 && temperature <= minTemp)) {
			temperature += amount;
		}

	}

	void syncTemperatureAnimation()
	{
		//Also add blue-ing effect on materials
		float percent = temperature / baseTemperature;

		animator.speed = percent;

	}

	void Homeostasis()
	{
		if (temperature > baseTemperature) {
			temperature -= homeostasisTendency;
		} else if (temperature < baseTemperature) {
			temperature += homeostasisTendency;
		}
	}

	//FROM OLD ENEMY SCRIPT, MIGHT NOT NEED
	void Update()
    {
		Homeostasis ();
		syncTemperatureAnimation ();

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

		if (PathStatus == NavMeshPathStatus.PathInvalid && !PathPending) {
			navAgent.SetDestination (target.position);
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

	void LateUpdate()
	{
		HPBarCanvas.rotation = HPBarRotation;
	}

}
