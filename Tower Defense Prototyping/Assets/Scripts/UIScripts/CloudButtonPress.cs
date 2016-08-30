using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CloudButtonPress : MonoBehaviour {

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
		tooltipMessage = "X - Cost $30 - Income +8 - Cloud Unit \n\nSuper-soldier infused with Mako energy";
		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

	}

	public void CloudSend()
	{
		if (kf.mode == 2) {
			kf.SendUnit("Cloud");
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
