using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PaladinButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "C - Cost $50 - Income +15 - Paladin Unit \n\nKnight with extra defense against lasers and light beams";

	}

	public override void ClickAction()
	{
		/*
		if (kf.Mode == 2) {
			kf.CheckSendUnit("Paladin");
		}*/

	}
}
