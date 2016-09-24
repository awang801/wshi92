using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OrbButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "X - Cost $20 - Orb Tower\n\nAD 1  RG 500  FR 0.66\nGood basic single target damage";

	}

	public override void ClickAction()
	{

		kf.BuildOrbTower ();

	}
}
