using UnityEngine;
using System.Collections;

public class WallButtonPress : MonoBehaviour {
	
	GameObject gm;
	KeyboardFunctions kf;
	MouseFunctions mf;

	void Awake () {
		gm = GameObject.Find ("GameManager");
		kf = gm.GetComponent<KeyboardFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
	}

	public void WallToggle()
	{
		if (kf.mode == 1) { 
			if (kf.SelectedTowerToBuild == false)
			{
				mf.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
			}
			Debug.Log ("BUILDING Z TOWER");
			kf.towerToBuild = "Wall";
			kf.SelectedTowerToBuild = true;
			mf.Mode = 1;

		}
	}
}
