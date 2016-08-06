using UnityEngine;
using System.Collections;

public class WallButtonPress : MonoBehaviour {
	
	GameObject gm;
	BuildingFunctions bf;
	MouseFunctions mf;

	void Awake () {
		gm = GameObject.Find ("GameManager");
		bf = gm.GetComponent<BuildingFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
	}

	public void WallToggle()
	{
		if (bf.isBuilding == true) { 
			if (bf.SelectedTowerToBuild == false)
			{
				mf.SelHighlight = ((GameObject)(Instantiate(Resources.Load("SelectionHighlight")))); //Creates green selection box
			}
			Debug.Log ("BUILDING Z TOWER");
			bf.towerToBuild = "Wall";
			bf.SelectedTowerToBuild = true;
			mf.Selecting = true;

		}
	}
}
