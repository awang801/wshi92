using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Cloud : Unit {

	protected override void Initialize ()
	{
		//Stats Initialization
		maxHealth = 10;
		health = 10;
		homeostasisTendency = 0.4f;
		baseTemperature = 98.6f;
		damageAmplifier = 1f;
		minTemp = 5f;
		maxTemp = 150f;
		temperature = baseTemperature;

		baseSpeed = 1.5f;

		killValue = 15;
	}

}
