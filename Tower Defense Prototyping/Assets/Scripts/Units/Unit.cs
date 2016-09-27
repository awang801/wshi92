using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour {

	//==============================================================
	//Navmesh Variables
	public bool HasPath = false;
	public bool PathPending = false;
	public bool PathStale = false;
	public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

	public Vector3 target;
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

	[SyncVar]
	float health;

	float damageAmplifier = 1f;

	float baseTemperature = 98.6f;

	[SyncVar]
	float temperature;

	float homeostasisTendency = 0.4f;
	float minTemp = 32f;
	float maxTemp = 150f;

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

	bool requestedID = false;
	public GameObject attackPlayer;
	public GameObject sendPlayer;


	Bank bank;

	//==============================================================



	//bool ableToFind;
	//bool calculating;



	void Awake()
	{
		
		//Navigation Initialization
		navAgent = GetComponent<NavMeshAgent> ();

		//Animator Initialization
		animator = GetComponent<Animator> ();
		angleHash = Animator.StringToHash ("Angle");
		speedHash = Animator.StringToHash ("Speed");
		deathHash = Animator.StringToHash ("Dead");
		finishHash = Animator.StringToHash ("Finish");

		//Visual Initialization
		HPBarCanvas = transform.GetChild (2);
		HPBar = transform.GetChild (2).GetChild (2);
		HPBarCanvas.Rotate (0, 180, 0);
		HPBarRotation = HPBarCanvas.rotation;
		navAgent.updateRotation = false;

		//Audio Initialization
		myAudioSource = GetComponent<AudioSource> ();
		deathSound = (AudioClip)Resources.Load ("Sounds/enemydeath");
		goodSound  = (AudioClip)Resources.Load ("Sounds/GoodBeep");
		badSound  = (AudioClip)Resources.Load ("Sounds/BadBeep");
	}

	void Start () {

		//Stats Initialization
		maxHealth = 10;
		health = 10;
		temperature = baseTemperature;

		//Bank Initialization

	}

	//Apply damage to unit
	public void Damage(float dmg)
	{		
		health -= damageAmplifier * dmg;

		if (health <= 0 && !isDying) {
			if (isServer) {
				RpcDie ();
			} else {
				HPBar.localScale = new Vector3 (0, 1, 1);
				Death ();
			}
		} else {
			HPBar.localScale = new Vector3 (Mathf.Clamp(health / maxHealth, 0f, 1f), 1, 1);
		}

	}

	[ClientRpc]
	public void RpcDie()
	{
		HPBar.localScale = new Vector3 (0, 1, 1);
		Death ();
	}

	[ClientRpc]
	public void RpcFinish()
	{
		Finish ();
	}

	//Set destination target
	[Command]
	public void CmdSetTarget(Vector3 tar)
	{
		RpcSetTarget (tar);
	}

	[ClientRpc]
	public void RpcSetTarget(Vector3 _target)
	{
		target = _target;
		navAgent.SetDestination (target);
	}

	[Command]
	public void CmdRequestPlayerID()
	{
		RpcSetPlayer (attackPlayer.transform.name, sendPlayer.transform.name);
	}

	[ClientRpc]
	public void RpcSetPlayer(string aplayerID, string splayerID)
	{
		attackPlayer = GameObject.Find (aplayerID);
		sendPlayer = GameObject.Find (splayerID);

		bank = attackPlayer.GetComponent<Bank>();
	}

	//Unit died before reaching target
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

	//Unit reached target
    public void Finish()
    {
		isDying = true;
		animator.SetTrigger (finishHash);
		if (attackPlayer.transform.name == "Player 4") {
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


	//==================================================================================
	//TEMPERATURE MECHANIC
	//==================================================================================

	public void addTemperature(float amount)
	{
		if (!(amount < 0 && temperature <= minTemp) && !(amount > 0 && temperature >= maxTemp)) {
			temperature += amount;
		} 

	}

	public void setTemperature(float amount)
	{
		
		temperature = amount;

	}

	void syncTemperature()
	{
		//NEED TO - Also add blue-ing effect on materials

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


	//==================================================================================
	//DAMAGE AMPLIFIER
	//==================================================================================

	public void addDamageAmplifier(float amount)
	{

		damageAmplifier += amount;

	}

	public void setDamageAmplifier(float amount)
	{

		damageAmplifier = amount;

	}

	//==================================================================================

	//==================================================================================
	void Update()
    {


		//Temperature Mechanic
		Homeostasis ();
		syncTemperature ();

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

		if (PathStatus == NavMeshPathStatus.PathInvalid && !PathPending || PathStatus == NavMeshPathStatus.PathPartial && !PathPending) {
			navAgent.SetDestination (target);
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

	//Update HP bar rotations
	void LateUpdate()
	{
		HPBarCanvas.rotation = HPBarRotation;
	}

}
