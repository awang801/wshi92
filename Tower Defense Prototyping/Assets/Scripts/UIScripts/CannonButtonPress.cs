using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CannonButtonPress : MonoBehaviour {

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

		tooltipMessage = "C - Cost $40 - Cannon\n\nAD 2  RG 600  FR 0.5\nStrong against clusters of enemies";
	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public void TowerToggle()
	{
		if (kf.mode == 1) {
			if (kf.SelectedTowerToBuild == false)
			{
				mf.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
			}
			Debug.Log ("BUILDING C TOWER");
			kf.towerToBuild = "CannonTower";
			kf.SelectedTowerToBuild = true;
			mf.Mode = 1;

		}
	}

	public void PlayHighlightSound()
	{
		gmAudioSource.PlayOneShot (highlightSFX);
	}
}
