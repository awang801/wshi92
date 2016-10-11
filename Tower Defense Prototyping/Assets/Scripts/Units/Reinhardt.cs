using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Reinhardt : Unit {

	//Ultimate
	//=========================================
	float ultCharge;
	float ultGainRatio;
	float ultPassiveGain;

	bool ultReady;

	//Animations
	int shieldingHash;


	//Audio
	AudioClip barrierActivated;

	//Shield
	//=========================================
	public Shield myShield;

	protected override void Initialize ()
	{
		//Stats Initialization
		maxHealth = 100;
		health = 100;
		homeostasisTendency = 0.4f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 32f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 0.75f;

		killValue = 50;

		ultGainRatio = 4f;
		ultPassiveGain = 0.2f;

		shieldingHash = Animator.StringToHash ("Shielding");

		barrierActivated = (AudioClip)(Resources.Load("Sounds/Barrier", typeof(AudioClip)));
		ShieldOff ();

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
		ShieldOff ();
		ultCharge = 0;
		ultReady = false;
	}

	protected override void OnFinish ()
	{
		ShieldOff ();
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

			ShieldOn ();

		}

	}

	public void ShieldOn()
	{
		RpcShieldOn ();
	}

	[ClientRpc]
	public void RpcShieldOn()
	{
		
		myAudioSource.PlayOneShot (barrierActivated);
		myShield.gameObject.SetActive (true);
		myShield.AnimateOn ();
		animator.SetBool (shieldingHash, true);
		ultCharge = 0f;

	}

	public void ShieldOff()
	{
		RpcShieldOff ();
	}

	[ClientRpc]
	public void RpcShieldOff()
	{

		myShield.gameObject.SetActive (false);
		animator.SetBool (shieldingHash, false);
		ultReady = false;
		baseSpeed = 0.75f;

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

	public void ShieldTakeDamage(float dmg)
	{

		if (isServer) {
			RpcShieldTakeDamage (dmg);
		}

	}

	[ClientRpc]
	void RpcShieldTakeDamage(float dmg)
	{

		myShield.TakeDamage (dmg);

	}

}
