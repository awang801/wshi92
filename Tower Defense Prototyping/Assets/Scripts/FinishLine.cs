using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	public GameObject playerToHurt;

	Lives lives;

	void Start () {
		lives = playerToHurt.GetComponent<Lives>();
    }

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
            other.gameObject.GetComponent<Unit>().Finish();
            lives.loseLife();
        } 
	}
}
