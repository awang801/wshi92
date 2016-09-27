using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems;

public class SendButtonPress : MonoBehaviour {

	public GameManager gm;

	GameObject myPlayer;
	KeyboardFunctions kf;
	MouseFunctions mf;
	GameObject panel;

	RectTransform panelTransform;

	GameObject potatoButtonObj;
	GameObject cloudButtonObj;

	Button potatoButton;
	Button cloudButton;

	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	string tooltipMessage;

	BuildButtonPress buildButton;

	// Use this for initialization
	void Awake () {
		
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();

		panel = GameObject.Find ("ButtonPanel");
		panelTransform = panel.GetComponent<RectTransform> ();

		potatoButtonObj = GameObject.Find ("PotatoButton");
		cloudButtonObj = GameObject.Find ("CloudButton");

		potatoButton = potatoButtonObj.GetComponent<Button> ();
		cloudButton = cloudButtonObj.GetComponent<Button> ();

		potatoButton.interactable = false;
		cloudButton.interactable = false;

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
			kf.CancelSend ();
		} 
		else if (kf.Mode == 1)
		{
			buildButton.BuildToggle ();
			potatoButton.interactable = true;
			cloudButton.interactable = true;
			kf.Send ();
		}
		else if (kf.Mode == 0)
		{
			kf.Send ();
			potatoButton.interactable = true;
			cloudButton.interactable = true;
		}
	}
		
}
