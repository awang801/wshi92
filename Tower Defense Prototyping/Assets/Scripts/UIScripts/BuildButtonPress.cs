using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems;

public class BuildButtonPress : MonoBehaviour {

	public GameManager gm;

	GameObject myPlayer;
	KeyboardFunctions kf;
	//MouseFunctions mf;
	GameObject cannonButtonObj;
	GameObject towerButtonObj;
	GameObject wallButtonObj;
	GameObject laserButtonObj;
	GameObject lightButtonObj;
	GameObject magicButtonObj;
	GameObject iceButtonObj;

	Button towerButton;
	Button cannonButton;
	Button wallButton;
	Button laserButton;
	Button lightButton;
	Button magicButton;
	Button iceButton;


	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	FadeObjectInOut tooltipFader;
	string tooltipMessage;

	SendButtonPress sendButton;


	// Use this for initialization
	void Awake () {
		

		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();


		//mf = gm.GetComponent<MouseFunctions> ();

		towerButtonObj = GameObject.Find ("OrbTowerButton");
		cannonButtonObj = GameObject.Find ("CannonTowerButton");
		wallButtonObj = GameObject.Find ("WallButton");
		laserButtonObj = GameObject.Find ("LaserTowerButton");
		lightButtonObj = GameObject.Find ("LightTowerButton");
		magicButtonObj = GameObject.Find ("MagicTowerButton");
		iceButtonObj = GameObject.Find ("IceTowerButton");

		towerButton = towerButtonObj.GetComponent<Button> ();
		cannonButton = cannonButtonObj.GetComponent<Button> ();
		wallButton = wallButtonObj.GetComponent<Button> ();
		iceButton = iceButtonObj.GetComponent<Button> ();
		laserButton = laserButtonObj.GetComponent<Button> ();
		lightButton = lightButtonObj.GetComponent<Button> ();
		magicButton = magicButtonObj.GetComponent<Button> ();

		towerButton.interactable = false;
		cannonButton.interactable = false;
		wallButton.interactable = false;
		iceButton.interactable = false;
		laserButton.interactable = false;
		lightButton.interactable = false;
		magicButton.interactable = false;


		sendButton = GameObject.Find("SendButtonText").GetComponent<SendButtonPress>();
		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipFader = tooltipText.GetComponentInParent<FadeObjectInOut> ();

		tooltipMessage = "B - Toggle Build Mode\n\nBuild walls and towers for defense!\n(Towers must be placed on walls)";
	}

	void Update()
	{
		if (myPlayer == null) {
			myPlayer = gm.MyLocalPlayer ();
		} else {
			if (kf == null)
			kf = myPlayer.GetComponent<KeyboardFunctions> ();
		}
	}

	public void PlayHighlightSound()
	{
		mainCamAudioSource.PlayOneShot (highlightSFX);
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;

		if (tooltipFader.visible == false) {
			tooltipFader.FadeIn (1f);
		}

	}

	public void HideToolTip()
	{
		tooltipText.text = "";

		if (tooltipFader.visible == true) {
			tooltipFader.FadeOut (1f);
		}
	}

	public void BuildToggle()
	{
		if (kf.Mode == 2) {
			sendButton.SendToggle ();
			towerButton.interactable = true;
			cannonButton.interactable = true;
			wallButton.interactable = true;
			iceButton.interactable = true;
			laserButton.interactable = true;
			lightButton.interactable = true;
			magicButton.interactable = true;
			kf.Build ();
		}
		else if (kf.Mode == 1) {
			towerButton.interactable = false;
			cannonButton.interactable = false;
			wallButton.interactable = false;
			iceButton.interactable = false;
			laserButton.interactable = false;
			lightButton.interactable = false;
			magicButton.interactable = false;
			kf.CancelBuild ();
		} else if (kf.Mode == 0){
			kf.Build ();
			towerButton.interactable = true;
			cannonButton.interactable = true;
			wallButton.interactable = true;
			iceButton.interactable = true;
			laserButton.interactable = true;
			lightButton.interactable = true;
			magicButton.interactable = true;
		}
	}


}
