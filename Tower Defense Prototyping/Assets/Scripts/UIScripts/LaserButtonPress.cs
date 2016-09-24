using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LaserButtonPress : ButtonPress {


	public override void Initialize()
	{

		tooltipMessage = "V - Cost $50 - Laser\n\nDPS   RG 600 \nNeeds to charge but does significant damage to enemies close together";

	}

	public override void ClickAction()
	{
		
		kf.BuildLaserTower ();

	}

}
