using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour {

	public GameManager gm;
	protected GameObject myPlayer;
	protected KeyboardFunctions kf;
	protected MouseFunctions mf;

	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	protected string tooltipMessage;

	void Awake () {
		
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();

		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		Initialize ();

	}

	void Update()
	{
		if (myPlayer == null) {
			myPlayer = gm.MyLocalPlayer ();
		} else {
			if (kf == null)
				kf = myPlayer.GetComponent<KeyboardFunctions> ();
			if (mf == null)
			mf = myPlayer.GetComponent<MouseFunctions> ();
		}
	}

	public virtual void Initialize()
	{
		
		tooltipMessage = "V - Cost $50 - Laser\n\nDPS   RG 600 \nNeeds to charge but does significant damage to enemies close together";

	}

	public void DisplayTooltip()
	{
		PlayHighlightSound ();

		tooltipText.text = tooltipMessage;
	}

	public virtual void ClickAction()
	{

		//Build Tower Here

	}

	public void PlayHighlightSound()
	{
		mainCamAudioSource.PlayOneShot (highlightSFX);
	}

}
