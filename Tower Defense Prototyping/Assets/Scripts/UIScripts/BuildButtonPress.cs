using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems;

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

	AudioSource gmAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	string tooltipMessage;

	SendButtonPress sendButton;


	// Use this for initialization
	void Awake () {
		
		gm = GameObject.Find ("GameManager");
		gmAudioSource = gm.GetComponent<AudioSource> ();

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

		sendButton = GameObject.Find("SendButtonText").GetComponent<SendButtonPress>();
		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipMessage = "B - Toggle Build Mode\n\nBuild walls and towers for defense!\n(Towers must be placed on walls)";
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void BuildToggle()
	{
		if (kf.mode == 2) {
			sendButton.SendToggle ();
			towerButton.interactable = true;
			cannonButton.interactable = true;
			wallButton.interactable = true;
			kf.Build ();
		}
		else if (kf.mode == 1) {
			towerButton.interactable = false;
			cannonButton.interactable = false;
			wallButton.interactable = false;
			kf.CancelBuild ();
		} else if (kf.mode == 0){
			kf.Build ();
			towerButton.interactable = true;
			cannonButton.interactable = true;
			wallButton.interactable = true;
		}
	}


}
