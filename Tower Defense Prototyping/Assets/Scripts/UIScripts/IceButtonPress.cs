using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class IceButtonPress : ButtonPress {

	public override void Initialize()
	{

		tooltipMessage = "F - Cost $50 - Ice\n\nSlows enemies hit but does negligible damage. Very short range.";

	}

	public override void ClickAction()
	{

		kf.BuildIceTower ();

	}
}
