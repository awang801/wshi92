using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Paladin : Unit {

	//Ultimate
	//=========================================
	float ultCharge;
	float ultGainRatio;
	float ultPassiveGain;

	bool ultReady;

	//Animations
	int spinningHash;

	bool isSpinning = false;


	//Audio
	AudioClip spinActivated;

	protected override void Initialize ()
	{
		//Stats Initialization
		maxHealth = 30;
		health = 30;
		homeostasisTendency = 0.4f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 32f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 1f;

		killValue = 15;

		ultGainRatio = 5f;
		ultPassiveGain = 1f;

		spinningHash = Animator.StringToHash ("Spinning");

		spinActivated = (AudioClip)(Resources.Load("Sounds/spinkick", typeof(AudioClip)));
		SpinOff ();

		isFemale = false;

	}

	protected override void OnDamageTaken (float dmg)
	{

		if (!ultReady) {
			GainUltCharge (dmg * ultGainRatio);
		}

	}

	protected override void OnDeath ()
	{
		SpinOff ();
		ultCharge = 0;
		ultReady = false;
	}

	protected override void OnFinish ()
	{
		SpinOff ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (!isServer)
			return;
		
		if (!ultReady) {
			GainPassiveUltCharge ();
		}

		if (ultReady && ultCharge >= 100f) {

			if (!isSpinning) {
				SpinOn ();
			}


		}

	}

	public void SpinOn()
	{
		RpcSpinOn ();
	}

	[ClientRpc]
	public void RpcSpinOn()
	{

		isSpinning = true;
		myAudioSource.PlayOneShot (spinActivated);
		animator.SetBool (spinningHash, true);
		ultCharge = 0f;

	}

	public void SpinOff()
	{
		RpcSpinOff ();
	}

	[ClientRpc]
	public void RpcSpinOff()
	{
		isSpinning = false;
		animator.SetBool (spinningHash, false);
		ultReady = false;

	}
		
	void GainUltCharge(float amt)
	{

		ultCharge += amt;

		if (ultCharge >= 100f) {
			
			ultCharge = 100f;
			ultReady = true;
		}

	}

	void GainPassiveUltCharge()
	{

		GainUltCharge (ultPassiveGain * Time.deltaTime);

	}

	protected override void OnAnimatorMove() //Access to Root Motion 
	{
		

		if (!isSpinning) {
			if (MixedMode && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Locomotion")) {
				transform.rotation = animator.rootRotation;
			}

			navAgent.velocity = animator.deltaPosition / Time.deltaTime;

		} else {
			
			navAgent.velocity = navAgent.desiredVelocity * 2f;

		}

	}

	protected override void Navigate()
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

}
