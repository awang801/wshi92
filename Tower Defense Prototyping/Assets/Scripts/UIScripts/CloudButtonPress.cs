using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CloudButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "X - Cost $30 - Income +8 - Cloud Unit \n\nSuper-soldier infused with Mako energy";

	}

	public override void ClickAction()
	{

		if (kf.Mode == 2) {
			kf.CheckSendUnit("Cloud");
		}

	}
}
