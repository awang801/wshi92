using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MercyButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "F - Cost $300 - Income +35 - Mercy Unit \n\nDoctor angel, with ability to resurrect nearby allies";

	}

	public override void ClickAction()
	{

		if (kf.Mode == 2) {
			kf.CheckSendUnit("Mercy");
		}

	}
}
