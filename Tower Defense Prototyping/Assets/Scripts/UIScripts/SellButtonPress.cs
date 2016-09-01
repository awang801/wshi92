using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SellButtonPress : MonoBehaviour {

	GameObject gm;
	KeyboardFunctions kf;
	BuildHandler bhandler;
	MouseFunctions mf;

	AudioSource gmAudioSource;
	AudioClip highlightSFX;

	float buttonHoldTime;
	float buttonThresholdTime = 2f;
	bool runTimer = false;

	public Text tooltipText;
	string tooltipMessage;

	void Awake () {
		gm = GameObject.Find ("GameManager");
		gmAudioSource = gm.GetComponent<AudioSource> ();
		kf = gm.GetComponent<KeyboardFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
		bhandler = gm.GetComponent<BuildHandler> ();

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
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
