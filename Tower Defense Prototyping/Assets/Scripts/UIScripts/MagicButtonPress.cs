using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MagicButtonPress : ButtonPress {


	public override void Initialize()
	{

		tooltipMessage = "H - Cost $50 - Magic\n\nAD 3  RG 600  FR 1\nStrong single target damage";

	}

	public override void ClickAction()
	{

		kf.BuildMagicTower ();

	}

}
