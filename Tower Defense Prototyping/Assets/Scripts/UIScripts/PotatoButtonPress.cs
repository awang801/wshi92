using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PotatoButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "Z - Cost $10 - Income +1 - Potato Unit \n\nMost basic unit. Delicious.";

	}

	public override void ClickAction()
	{

		if (kf.Mode == 2) {
			kf.CheckSendUnit("Potato");
		}

	}
}
