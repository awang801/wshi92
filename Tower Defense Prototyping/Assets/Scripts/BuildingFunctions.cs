using UnityEngine;
using System.Collections;

public class BuildingFunctions : MonoBehaviour {

	MouseFunctions mFunc;
	public bool isBuilding;

	string towerToBuild;

	bool selectedTowerToBuild;

	void Awake()
	{
		mFunc = this.gameObject.GetComponent<MouseFunctions> ();
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		CheckButtons ();

	}

	void CheckButtons()
	{
		if (Input.GetButtonDown ("Cancel")) {

			if (selectedTowerToBuild == true) {

				Debug.Log ("CANCEL SELECTED TOWER");
				selectedTowerToBuild = false;
				mFunc.Selecting = false;
				Destroy (mFunc.SelHighlight);

			} else if (isBuilding == true && selectedTowerToBuild == false) {

				Debug.Log ("CANCEL BUILDING");
				isBuilding = false;

			}
				
		} else if (Input.GetButtonDown ("Build")) {
			
			if (isBuilding == false) {
				
				isBuilding = true;
				Debug.Log ("BUILDING START");

			} else {

			}

		} else if (Input.GetButtonDown ("Z")) {
			
			if (isBuilding == true) {

				Debug.Log ("BUILDING Z TOWER");
				towerToBuild = "Wall";
				SelectedTowerToBuild = true;
				mFunc.Selecting = true;

				mFunc.SelHighlight = ((GameObject)(Instantiate (Resources.Load ("SelectionHighlight"))));
					
			}
		} else if (Input.GetButtonDown ("X")) {

				if (isBuilding == true) {

					Debug.Log ("BUILDING X TOWER");
					towerToBuild = "Tower";
					SelectedTowerToBuild = true;
					mFunc.Selecting = true;

					mFunc.SelHighlight = ((GameObject)(Instantiate (Resources.Load ("SelectionHighlight"))));

				}
			}


	}

	public bool SelectedTowerToBuild
	{
		get{
			return selectedTowerToBuild;
		}
		set{
			selectedTowerToBuild = value;
		}
	}

	public string TowerToBuild

	{
		get{
			return towerToBuild;
		}
		set{
			towerToBuild = value;
		}
	}

}

