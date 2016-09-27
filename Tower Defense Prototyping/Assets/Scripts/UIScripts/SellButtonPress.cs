using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SellButtonPress : MonoBehaviour {

	public GameManager gm;

	GameObject myPlayer;
	KeyboardFunctions kf;
	BuildHandler bhandler;
	MouseFunctions mf;

	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	float buttonHoldTime;
	float buttonThresholdTime = 2f;
	bool runTimer = false;

	public Text tooltipText;
	string tooltipMessage;

	void Awake () {
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();

		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipMessage = "Sell this structure for its sell value";
	}

	void Update()
	{
		if (myPlayer == null) 
		{
			
			myPlayer = gm.MyLocalPlayer ();

		} 

		else {
			if (kf == null) 
			{
				kf = myPlayer.GetComponent<KeyboardFunctions> ();
			}

			if (mf == null) 
			{
				mf = myPlayer.GetComponent<MouseFunctions> ();
			} 

			if (bhandler == null)
			{
				bhandler = myPlayer.GetComponent<BuildHandler> ();
			}

		}

		if (runTimer == true) {
			buttonHoldTime += Time.deltaTime;

			if (buttonHoldTime >= buttonThresholdTime) {
				SellSelected ();
				runTimer = false;
				buttonHoldTime = 0;
			}
		}
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void SellSelected()
	{
		bhandler.SellSelection ();
	}

	public void StartTiming()
	{
		buttonHoldTime = 0f;
		runTimer = true;
	}

	public void StopTiming()
	{
		runTimer = false;
	}

	public void PlayHighlightSound()
	{
		mainCamAudioSource.PlayOneShot (highlightSFX);
	}
}
