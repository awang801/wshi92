using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly

public class BuildButtonPress : MonoBehaviour {

	GameObject gm;
	BuildingFunctions bf;
	MouseFunctions mf;
	GameObject panel;
	RectTransform panelTransform;
	GameObject towerButton;
	GameObject wallButton;

	float panelMaxX;
	float panelMinX;

	public float changeDirection;


	// Use this for initialization
	void Awake () {
		gm = GameObject.Find ("GameManager");
		bf = gm.GetComponent<BuildingFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
		panel = GameObject.Find ("Panel");
		panelTransform = panel.GetComponent<RectTransform> ();
		towerButton = GameObject.Find ("OrbTowerButton");
		wallButton = GameObject.Find ("WallButton");
		towerButton.SetActive (false);
		wallButton.SetActive (false);
		panelMaxX = 190;
		panelMinX = 70;
	}

	void Update()
	{
		panelSize ();
	}

	public void BuildToggle()
	{
		if (bf.isBuilding == true) {
			bf.CancelBuild ();
			changeDirection = -1;
		} else {
			bf.Build ();
			changeDirection = 1;
			towerButton.SetActive (true);
			wallButton.SetActive (true);
		}
	}

	void panelSize()
	{
		if (changeDirection > 0) {
			if (panelTransform.sizeDelta.x < panelMaxX) {
				panelTransform.sizeDelta = new Vector2 (panelTransform.sizeDelta.x + 10f, panelTransform.sizeDelta.y);
				panelTransform.position = new Vector3 (panelTransform.position.x + 5f, panelTransform.position.y, panelTransform.position.z);
				wallButton.transform.Translate (Vector3.right * (5.5f));
				towerButton.transform.Translate (Vector3.right * (11f));
			} else {
				changeDirection = 0;
			}
		} else if (changeDirection < 0) {
			if (panelTransform.sizeDelta.x > panelMinX) {
				panelTransform.sizeDelta = new Vector2(panelTransform.sizeDelta.x - 10f ,panelTransform.sizeDelta.y);
				panelTransform.position = new Vector3 (panelTransform.position.x - 5f, panelTransform.position.y, panelTransform.position.z);
				wallButton.transform.Translate (Vector3.right * (-5.5f));
				towerButton.transform.Translate (Vector3.right * (-11f));
			} else {
				changeDirection = 0;
				towerButton.SetActive (false);
				wallButton.SetActive (false);
			}
		}
	}


}
