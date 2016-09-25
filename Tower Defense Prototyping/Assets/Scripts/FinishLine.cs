using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	GameManager gm;
	public GameObject playerToHurt;
	public GameObject otherPlayer;

	string playerToHurtId;
	string otherPlayerId;

	Lives lives;

	void Awake()
	{
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
            other.gameObject.GetComponent<Unit>().Finish();
            lives.loseLife();

			if (lives.getLives() <= 0) {
				gm.PlayerLoseWin (playerToHurtId, otherPlayerId);
			}

        } 
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
