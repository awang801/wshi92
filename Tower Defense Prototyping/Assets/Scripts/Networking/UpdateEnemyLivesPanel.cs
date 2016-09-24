using UnityEngine;
using System.Collections;

public class UpdateEnemyLivesPanel : MonoBehaviour {

	public bool hasSetObject = false;
	GameManager gm;
	// Use this for initialization
	void Awake () {
		
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();

	}
	
	// Update is called once per frame
	void Update () {
	
		if (gm.playerNumber == 2) {



		}

	}
}
