using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Vampire : Unit {

	//Ultimate
	//=========================================
	float ultCharge;
	float ultGainRatio;
	float ultPassiveGain;

	bool ultReady;

	//Animations
	int drainingHash;

	public GameObject castParticle;

	public Drain myDrainer;

	AudioClip drainSFX;

	public GameObject drainBullet;

	protected override void Initialize ()
	{
		//Stats Initialization
		maxHealth = 50;
		health = 50;
		homeostasisTendency = 0.6f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 32f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 0.9f;

		killValue = 25;

		ultGainRatio = 1f;
		ultPassiveGain = 8f;

		drainSFX = (AudioClip)(Resources.Load("Sounds/VampireDrain", typeof(AudioClip)));

		drainingHash = Animator.StringToHash ("Draining");

		isFemale = false;

	}

	protected override void OnDamageTaken (float dmg)
	{
		if (!ultReady) {
			GainUltCharge (dmg * ultGainRatio);
		}

	}

	protected override void OnPlayerSet ()
	{

		myDrainer.SetOwnerID (sendPlayer.name);

	}

	protected override void OnDeath ()
	{
		damageAmplifier = 1f;
		ultReady = false;
		ultCharge = 0;

	}

	protected override void OnFinish ()
	{



	}

	void CastDrain()
	{
		RpcDrain ();
	}

	[ClientRpc]
	void RpcDrain()
	{
		ultReady = false;
		ultCharge = 0;
		animator.SetTrigger (drainingHash);
		myAudioSource.PlayOneShot (drainSFX);
		myDrainer.CastDrain ();
		Instantiate (castParticle, transform, false);
	}

	protected override void Update ()
	{
		base.Update ();

		if (!isServer)
			return;

		if (!isDying) {
			if (!ultReady) {
				GainPassiveUltCharge ();
			}

			if (ultReady) {

				CastDrain ();

			}
		}

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

	[ClientRpc]
	public void RpcDrainUnit(NetworkInstanceId unitId)
	{

		GameObject findUnit = null;
		Unit unitToDrain;

		if (isServer) {
			findUnit = NetworkServer.FindLocalObject(unitId);
		} else if (isClient) {
			findUnit = ClientScene.FindLocalObject (unitId);
		}

		if (findUnit != null) {
			
			unitToDrain = findUnit.GetComponent <Unit> ();

			unitToDrain.Damage (5f);
			damageAmplifier = damageAmplifier - 0.1f;

			Bullet newBullet = ((GameObject)(Instantiate(drainBullet, unitToDrain.transform.position, drainBullet.transform.rotation))).GetComponent<Bullet>();
			newBullet.Setup(this, -5f, 5f);


		} else {
			Debug.Log ("Error: Drain Unit Not Found");
		}

	}

}
