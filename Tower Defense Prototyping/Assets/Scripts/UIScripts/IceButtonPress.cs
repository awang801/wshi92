using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class IceButtonPress : MonoBehaviour {

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

		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		tooltipMessage = "F - Cost $50 - Ice\n\nSlows enemies hit but does negligible damage. Very short range.";
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void TowerToggle()
	{
		kf.BuildIceTower ();
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
