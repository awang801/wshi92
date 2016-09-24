using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class LightButtonPress : ButtonPress {


	public override void Initialize()
	{

		tooltipMessage = "H - Cost $80 - Light\n\nDoes TONS OF DAMAGE to units that are slowed.";

	}

	public override void ClickAction()
	{

		kf.BuildLightTower ();

	}
}
