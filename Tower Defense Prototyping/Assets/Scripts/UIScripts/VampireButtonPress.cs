using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VampireButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "F - Cost $75 - Income +12 - Paladin Unit \n\nKnight with extra defense against lasers and light beams";

	}

	public override void ClickAction()
	{
		/*
		if (kf.Mode == 2) {
			kf.CheckSendUnit("Vampire");
		}*/

	}
}
