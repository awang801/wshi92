using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WallButtonPress : ButtonPress {
	
	public override void Initialize()
	{

		tooltipMessage = "Z - Toggle Build Wall Mode\n\nCreate mazes to slow down your enemies!";

	}

	public override void ClickAction()
	{

		kf.BuildWall ();

	}
}
