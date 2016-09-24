using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CannonButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "C - Cost $40 - Cannon\n\nAD 2  RG 600  FR 0.5\nStrong against clusters of enemies";

	}

	public override void ClickAction()
	{

		kf.BuildCannonTower ();

	}
}
