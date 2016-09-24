using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour {

	protected GameObject myPlayer;
	protected KeyboardFunctions kf;
	protected MouseFunctions mf;

	AudioSource mainCamAudioSource;
	AudioClip highlightSFX;

	public Text tooltipText;
	protected string tooltipMessage;

	void Awake () {
		
		myPlayer = transform.root.gameObject;
		mainCamAudioSource = Camera.main.GetComponent<AudioSource> ();
		kf = myPlayer.GetComponent<KeyboardFunctions> ();
		mf = myPlayer.GetComponent<MouseFunctions> ();

		highlightSFX = Resources.Load<AudioClip> ("Sounds/UI/UIMouseOverSound");

		Initialize ();

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
