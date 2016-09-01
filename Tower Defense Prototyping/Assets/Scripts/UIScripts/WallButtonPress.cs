using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WallButtonPress : MonoBehaviour {
	
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

		tooltipMessage = "Z - Toggle Build Wall Mode\n\nCreate mazes to slow down your enemies!";
	}

	public void WallToggle()
	{
		if (kf.mode == 1) { 
			if (kf.SelectedObjectToBuild == false)
			{
				mf.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
			}
			Debug.Log ("BUILDING Z TOWER");
			kf.ObjectToBuild = "Wall";
			kf.SelectedObjectToBuild = true;
			mf.Mode = 1;

		}
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
