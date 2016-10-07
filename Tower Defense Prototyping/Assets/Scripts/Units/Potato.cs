using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Potato : Unit {

	protected override void Initialize ()
	{
		//Stats Initialization
		maxHealth = 5;
		health = 5;
		homeostasisTendency = 0.4f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 32f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 1f;

		killValue = 3;

		isFemale = false;
	}

}
