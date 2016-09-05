using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class LaserButtonPress : MonoBehaviour {

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

		tooltipMessage = "V - Cost $50 - Laser\n\nDPS   RG 600 \nNeeds to charge but does significant damage to enemies close together";
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void TowerToggle()
	{
		kf.BuildLaserTower ();
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
