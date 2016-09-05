using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MagicButtonPress : MonoBehaviour {

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

		tooltipMessage = "H - Cost $50 - Magic\n\nAD 3  RG 600  FR 1\nStrong single target damage";
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void TowerToggle()
	{
		kf.BuildMagicTower ();
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
