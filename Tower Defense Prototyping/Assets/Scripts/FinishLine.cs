using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	public GameObject playerToHurt;

	Lives lives;


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
            other.gameObject.GetComponent<Unit>().Finish();
            lives.loseLife();
        } 
	}

	public void SetPlayerToHurt(GameObject player)
	{
		playerToHurt = player;
		lives = playerToHurt.GetComponent<Lives>();
	}
}
