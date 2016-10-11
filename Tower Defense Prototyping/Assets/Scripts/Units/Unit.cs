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

	protected NavMeshAgent navAgent = null;
	//==============================================================
	//Animation
	protected Animator animator = null;

	protected int angleHash;
	protected int speedHash;
	protected int deathHash;
	protected int finishHash;
	protected int beingResurrectedHash;

	protected float smoothAngle = 0f;
	public bool MixedMode = false;

	//==============================================================
	//Stats

	protected float maxHealth;

	protected float health;

	protected float damageAmplifier;

	protected float baseTemperature;

	protected float temperature;

	protected float homeostasisTendency;
	protected float minTemp;
	protected float maxTemp;

	protected int killValue;

	protected float baseSpeed;

	public bool isDying;
	public bool isDead = false;
	public bool isBeingResurrected = false;

	//==============================================================
	//Audio

	protected AudioSource myAudioSource;
	protected AudioClip[] deathSounds;
	protected bool isFemale;

	//===============================================================
	//Visuals
	protected Transform HPBarCanvas;
	Transform HPBar;
	public Quaternion HPBarRotation;

	//===============================================================
	//Owner and References

	public GameObject attackPlayer;
	public GameObject sendPlayer;



	Bank bank;

	//==============================================================

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
		beingResurrectedHash = Animator.StringToHash ("Resurrected");

		//Visual Initialization
		HPBarCanvas = transform.GetChild (2);
		HPBar = transform.GetChild (2).GetChild (2);
		HPBarCanvas.Rotate (0, 180, 0);
		HPBarRotation = HPBarCanvas.rotation;
		navAgent.updateRotation = false;

		//Audio Initialization
		myAudioSource = GetComponent<AudioSource> ();


	}

	protected virtual void Start () {
			
		Initialize ();
		LoadSounds ();

	}

	protected virtual void Initialize()
	{
		//Stats Initialization

		maxHealth = 10;
		health = 10;
		homeostasisTendency = 0.4f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 32f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 1f;

		killValue = 3;
	}

	void LoadSounds()
	{
		deathSounds = new AudioClip[3];
		if (isFemale) {
			deathSounds[0] = (AudioClip)Resources.Load ("Sounds/Death/FemaleDeath1");
			deathSounds[1] = (AudioClip)Resources.Load ("Sounds/Death/FemaleDeath2");
			deathSounds[2] = (AudioClip)Resources.Load ("Sounds/Death/FemaleDeath3");
		} else {
			deathSounds[0] = (AudioClip)Resources.Load ("Sounds/Death/MaleDeath1");
			deathSounds[1] = (AudioClip)Resources.Load ("Sounds/Death/MaleDeath2");
			deathSounds[2] = (AudioClip)Resources.Load ("Sounds/Death/MaleDeath3");
		}
	}

	//Apply damage to unit
	public void Damage(float dmg)
	{		
		if (isServer) {
			RpcDamage (dmg);
		}

	}

	[ClientRpc]
	void RpcDamage(float dmg)
	{
		if (!isDying) {
			health -= damageAmplifier * dmg;
			OnDamageTaken (dmg);

			if (health <= 0 && !isDying) {
				//if (isServer) {
				//	RpcDie ();
				//} else {
				health = 0;
				HPBar.localScale = new Vector3 (0, 1, 1);
					
				Death ();
				//}
			} else {
				HPBar.localScale = new Vector3 (Mathf.Clamp(health / maxHealth, 0f, 1f), 1, 1);
			}
		}
	}

	public void Heal(float amt)
	{
		if (isServer) {
			RpcHeal (amt);
		}

	}

	[ClientRpc]
	void RpcHeal(float amt)
	{
		health += amt;

		if (health >= maxHealth) {
			HPBar.localScale = new Vector3 (1, 1, 1);
			health = maxHealth;
			//}
		} else {
			HPBar.localScale = new Vector3 (Mathf.Clamp(health / maxHealth, 0f, 1f), 1, 1);
		}
	}


	protected virtual void OnDamageTaken(float dmg)
	{
		//Nothing
	}

	protected virtual void OnDeath()
	{
		//Nothing
	}

	protected virtual void OnFinish()
	{
		//Nothing
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

		OnPlayerSet ();
	}

	protected virtual void OnPlayerSet()
	{

	}

	//Unit died before reaching target
	void Death()
	{
		isDying = true;
		animator.speed = 1f;
		animator.SetTrigger (deathHash);
		PlayRandomDeathSound ();
		HPBarCanvas.gameObject.SetActive (false);

		//Death Animation Particle Effect
		//Instantiate (Resources.Load ("Enemies/EnemyDeath"), transform.position, transform.rotation);

		//Destroy (gameObject, animator.GetCurrentAnimatorClipInfo(0).Length + 1);
		//myFader.FadeOut();
		Invoke("temporaryWorkAround", animator.GetCurrentAnimatorClipInfo(0).Length + 10);
		bank.addMoney(killValue);

		OnDeath ();
    }

	void PlayRandomDeathSound()
	{

		int randomClip = Random.Range (0, 2);
		myAudioSource.PlayOneShot (deathSounds [randomClip]);

	}

	public void ResurrectMe()
	{
		isBeingResurrected = true;
		Heal (maxHealth);

		CancelInvoke ();
		animator.SetTrigger (beingResurrectedHash);
		animator.speed = 2f;

		Invoke ("SetAlive", 2f);

	}

	void SetAlive()
	{
		HPBarCanvas.gameObject.SetActive (true);
		isDying = false;
		isDead = false;
		isBeingResurrected = false;
	}

	void temporaryWorkAround()
	{
		isDead = true;
		gameObject.SetActive (false);
	}

	//Unit reached target
    public void Finish()
    {
		OnFinish ();
		isDead = true;
		isDying = true;
		animator.speed = 1f;
		animator.SetTrigger (finishHash);
		//myFader.FadeOut();
		Invoke("temporaryWorkAround", animator.GetCurrentAnimatorClipInfo(0).Length + 1);

		//Finish Animation Particle Effect
        //Instantiate(Resources.Load("Enemies/EnemyDeath"), transform.position, transform.rotation); //Play some sort of teleport animation here

		//Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0).Length);
    }

	protected virtual void OnAnimatorMove() //Access to Root Motion 
	{
		if (MixedMode && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Locomotion")) {
			transform.rotation = animator.rootRotation;
		}


		navAgent.velocity = animator.deltaPosition / Time.deltaTime;
	}


	//==================================================================================
	//TEMPERATURE MECHANIC
	//==================================================================================

	[ClientRpc]
	public void RpcAddTemperature(float amount)
	{
		if (!(amount < 0 && temperature <= minTemp) && !(amount > 0 && temperature >= maxTemp)) {
			temperature += amount;
		} 

	}

	public void addTemperature(float amount)
	{
		if (isServer) {
			RpcAddTemperature (amount);
		}
	}

	public void setTemperature(float amount)
	{
		
		temperature = amount;

	}

	void syncTemperature()
	{
		//NEED TO - Also add blue-ing effect on materials

		float percent = temperature / baseTemperature * baseSpeed;

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
	protected virtual void Update()
    {

		if (!isDying) {
			//Temperature Mechanic
			Homeostasis ();
			syncTemperature ();

			//Navigation
			Navigate ();
		}
	

		//**************************************************************************************
		//OLD CODE - Testing how to prevent enemies from stopping everytime a new wall is built
		//======================================================================================

        /*
         * if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid)
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

	protected virtual void Navigate()
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

		if (PathStatus == NavMeshPathStatus.PathInvalid && !PathPending || PathStatus == NavMeshPathStatus.PathPartial && !PathPending) {
			navAgent.SetDestination (target);
		}
	}

	void LateUpdate()
	{
		HPBarCanvas.rotation = HPBarRotation;
	}

}
