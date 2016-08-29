using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly

public class BuildButtonPress : MonoBehaviour {

	GameObject gm;
	KeyboardFunctions kf;
	MouseFunctions mf;
	GameObject panel;
	RectTransform panelTransform;
	GameObject cannonButtonObj;
	GameObject towerButtonObj;
	GameObject wallButtonObj;

	Button towerButton;
	Button cannonButton;
	Button wallButton;

	float panelMaxX;
	float panelMinX;

	public float changeDirection;


	// Use this for initialization
	void Awake () {
		gm = GameObject.Find ("GameManager");
		kf = gm.GetComponent<KeyboardFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
		panel = GameObject.Find ("ButtonPanel");
		panelTransform = panel.GetComponent<RectTransform> ();
		towerButtonObj = GameObject.Find ("OrbTowerButton");
		cannonButtonObj = GameObject.Find ("CannonTowerButton");
		wallButtonObj = GameObject.Find ("WallButton");
		towerButton = towerButtonObj.GetComponent<Button> ();
		cannonButton = cannonButtonObj.GetComponent<Button> ();
		wallButton = wallButtonObj.GetComponent<Button> ();
		towerButton.interactable = false;
		cannonButton.interactable = false;
		wallButton.interactable = false;
		panelMaxX = 190;
		panelMinX = 70;
	}

	void Update()
	{
		panelSize ();
	}

	public void BuildToggle()
	{
		if (kf.mode == 1) {
			towerButton.interactable = false;
			cannonButton.interactable = false;
			wallButton.interactable = false;
			kf.CancelBuild ();
		} else {
			kf.Build ();
			towerButton.interactable = true;
			cannonButton.interactable = true;
			wallButton.interactable = true;
		}
	}

	void panelSize()
	{
		/*if (changeDirection > 0) {
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
		}*/
	}


}
