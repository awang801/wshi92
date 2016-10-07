using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems;

public class SendButtonPress : MonoBehaviour {

	public GameManager gm;

	GameObject myPlayer;
	KeyboardFunctions kf;
	MouseFunctions mf;

	GameObject potatoButtonObj;
	GameObject cloudButtonObj;
	GameObject paladinButtonObj;
	GameObject vampireButtonObj;
	GameObject mercyButtonObj;
	GameObject reinhardtButtonObj;


	Button potatoButton;
	Button cloudButton;
	Button paladinButton;
	Button vampireButton;
	Button mercyButton;
	Button reinhardtButton;

	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	string tooltipMessage;

	BuildButtonPress buildButton;

	// Use this for initialization
	void Awake () {
		
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();

		potatoButtonObj = GameObject.Find ("PotatoButton");
		cloudButtonObj = GameObject.Find ("CloudButton");
		paladinButtonObj = GameObject.Find ("PaladinButton");
		vampireButtonObj = GameObject.Find ("VampireButton");
		mercyButtonObj = GameObject.Find ("MercyButton");
		reinhardtButtonObj = GameObject.Find ("ReinhardtButton");

		potatoButton = potatoButtonObj.GetComponent<Button> ();
		cloudButton = cloudButtonObj.GetComponent<Button> ();
		paladinButton = paladinButtonObj.GetComponent<Button> ();
		vampireButton = vampireButtonObj.GetComponent<Button> ();
		mercyButton = mercyButtonObj.GetComponent<Button> ();
		reinhardtButton = reinhardtButtonObj.GetComponent<Button> ();

		potatoButton.interactable = false;
		cloudButton.interactable = false;
		paladinButton.interactable = false;
		vampireButton.interactable = false;
		mercyButton.interactable = false;
		reinhardtButton.interactable = false;


		buildButton = GameObject.Find("BuildButtonText").GetComponent<BuildButtonPress>();
		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipMessage = "T - Toggle Send Mode\n\nSend units to build your income and attack the enemy!";
	}

	void Update()
	{
		if (myPlayer == null) {
			myPlayer = gm.MyLocalPlayer ();
		} else {
			if (kf == null)
			kf = myPlayer.GetComponent<KeyboardFunctions> ();

			if (mf == null)
			mf = myPlayer.GetComponent<MouseFunctions> ();
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
	}
		
	public void SendToggle()
	{
		if (kf.Mode == 2) {
			potatoButton.interactable = false;
			cloudButton.interactable = false;
			paladinButton.interactable = false;
			vampireButton.interactable = false;
			mercyButton.interactable = false;
			reinhardtButton.interactable = false;
			kf.CancelSend ();
		} 
		else if (kf.Mode == 1)
		{
			buildButton.BuildToggle ();
			potatoButton.interactable = true;
			cloudButton.interactable = true;
			paladinButton.interactable = true;
			vampireButton.interactable = true;
			mercyButton.interactable = true;
			reinhardtButton.interactable = true;
			kf.Send ();
		}
		else if (kf.Mode == 0)
		{
			kf.Send ();
			potatoButton.interactable = true;
			cloudButton.interactable = true;
			paladinButton.interactable = true;
			vampireButton.interactable = true;
			mercyButton.interactable = true;
			reinhardtButton.interactable = true;
		}
	}
		
}
