using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Mercy : Unit {

	//Ultimate
	//=========================================
	float ultCharge;
	float ultGainRatio;
	float ultPassiveGain;

	bool ultReady;

	//Animations
	int resurrectingHash;

	public GameObject castParticle;

	public HealBot myHealBot;

	AudioClip heroesNeverDie;

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

		killValue = 50;

		ultGainRatio = 1.5f;
		ultPassiveGain = 3f;

		heroesNeverDie = (AudioClip)(Resources.Load("Sounds/HeroesNeverDie", typeof(AudioClip)));

		resurrectingHash = Animator.StringToHash ("Resurrecting");

		isFemale = true;

	}

	protected override void OnDamageTaken (float dmg)
	{
		if (!ultReady) {
			GainUltCharge (dmg * ultGainRatio);
		}

	}

	protected override void OnPlayerSet ()
	{

		myHealBot.SetOwnerID (sendPlayer.name);

	}

	protected override void OnDeath ()
	{

		ultReady = false;
		ultCharge = 0;

	}

	protected override void OnFinish ()
	{



	}

	void Resurrect()
	{

		ultReady = false;
		ultCharge = 0;
		animator.SetTrigger (resurrectingHash);
		myAudioSource.PlayOneShot (heroesNeverDie);
		myHealBot.CastResurrect ();
		Instantiate (castParticle, transform, false);


	}

	protected override void Update ()
	{
		base.Update ();

		if (!isDying) {
			if (!ultReady) {
				GainPassiveUltCharge ();
			}

			if (ultReady) {

				Resurrect ();

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

}
