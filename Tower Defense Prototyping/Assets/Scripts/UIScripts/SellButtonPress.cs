using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SellButtonPress : MonoBehaviour {

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
		myPlayer = transform.root.gameObject;
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();

		kf = myPlayer.GetComponent<KeyboardFunctions> ();
		mf = myPlayer.GetComponent<MouseFunctions> ();
		bhandler = myPlayer.GetComponent<BuildHandler> ();

		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipMessage = "Sell this structure for its sell value";
	}

	void Update()
	{
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
