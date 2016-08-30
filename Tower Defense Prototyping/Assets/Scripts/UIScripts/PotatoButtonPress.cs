using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PotatoButtonPress : MonoBehaviour {

	GameObject gm;
	KeyboardFunctions kf;
	MouseFunctions mf;

	AudioSource gmAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	string tooltipMessage;

	void Awake () {
		gm = GameObject.Find ("GameManager");
		gmAudioSource = gm.GetComponent<AudioSource> ();
		kf = gm.GetComponent<KeyboardFunctions> ();
		mf = gm.GetComponent<MouseFunctions> ();
		tooltipMessage = "Z - Cost $10 - Income +1 - Potato Unit \n\nMost basic unit. Delicious.";
		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

	}

	public void PotatoSend()
	{
		if (kf.mode == 2) {
			kf.SendUnit("Potato");
		}
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
}
