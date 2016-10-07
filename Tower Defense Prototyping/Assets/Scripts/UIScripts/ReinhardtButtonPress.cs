using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReinhardtButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "G - Cost $500 - Income +50 - Reinhardt Unit \n\nTanky unit with shield ability\n\nBlocks projectiles";

	}

	public override void ClickAction()
	{

		if (kf.Mode == 2) {
			kf.CheckSendUnit("Reinhardt");
		}

	}
}
