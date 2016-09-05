using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OrbButtonPress : MonoBehaviour {

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

		tooltipMessage = "X - Cost $20 - Orb Tower\n\nAD 1  RG 500  FR 0.66\nGood basic single target damage";
	}

	public void TowerToggle()
	{
		kf.BuildOrbTower ();
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
