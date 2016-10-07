using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	GameManager gm;

	public GameObject playerToHurt;
	public GameObject otherPlayer;

	string playerToHurtId;
	string otherPlayerId;
	string playerInstanceId;

	AudioSource mainAudioSource;
	AudioClip goodSound;
	AudioClip badSound;

	Lives lives;

	void Awake()
	{
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();

		mainAudioSource = Camera.main.GetComponent<AudioSource> ();
		goodSound  = (AudioClip)Resources.Load ("Sounds/GoodBeep");
		badSound  = (AudioClip)Resources.Load ("Sounds/BadBeep");
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
			Unit tempUnit = other.gameObject.GetComponent<Unit> ();
			if (!tempUnit.isDying) {
				tempUnit.Finish();
				lives.loseLife();

				if (lives.getLives() <= 0) {
					gm.PlayerLoseWin (playerToHurtId, otherPlayerId);
				}

				if (playerToHurtId.Equals (playerInstanceId)) {
					mainAudioSource.PlayOneShot (badSound);
				} else {
					mainAudioSource.PlayOneShot (goodSound);
				}


			}
            

        } 
	}

	public void SetMyPlayer(string playerID)
	{
		playerInstanceId = playerID;
	}

	public void SetPlayerToHurt(GameObject player, string playerID)
	{
		playerToHurt = player;
		lives = playerToHurt.GetComponent<Lives>();
		playerToHurtId = playerID;
	}

	public void setOtherPlayer(GameObject player, string playerID)
	{
		otherPlayer = player;
		otherPlayerId = playerID;
	}
}
